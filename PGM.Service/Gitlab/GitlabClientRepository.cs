using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.MergeRequests.Responses;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;
using GitLabApiClient.Models.Users.Responses;
using Newtonsoft.Json;
using NLog;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public class GitlabClientRepository : IGitlabClientRepository
    {
        private readonly GitLabClient _client;
        private PGMSetting Settings => _pgmSettingManagerService.CurrentSettings;
        private readonly IPgmSettingManagerService _pgmSettingManagerService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GitlabClientRepository(IPgmSettingManagerService pgmSettingManagerService)
        {
            _pgmSettingManagerService = pgmSettingManagerService;
            if (Settings != null)
            {
                _client = new GitLabClient("https://gitlab.com/", Settings.GitLabApiKey ?? "");
            }
        }
        
        public async Task PostMergeRequest(string branch, string mrTitle, GitlabIssue issue,
            GitlabProject currentProject)
        {
            CreateMergeRequest request = GetMergeRequestInfo(branch, mrTitle, issue.Id.ToString());

            try
            {
                await _client.MergeRequests.CreateAsync(currentProject.Id, request);
            }
            catch (JsonSerializationException e)
            {
                Logger.Error(e);
            }
            catch (AggregateException e)
            {
                Logger.Error(e);
            }
        }

        public async Task ValidateMergeRequest(GitlabIssue issue, GitlabProject currentProject)
        {
            MergeRequest mergeRequest = await GetMergeRequestFromCurrentIssue(issue, currentProject);
            await _client.MergeRequests.AcceptAsync(currentProject.Id, mergeRequest.Iid, new AcceptMergeRequest
            {
                RemoveSourceBranch = true,
                MergeWhenPipelineSucceeds = false,
                MergeCommitMessage = $"[{issue.Id}] - {issue.Title}"
            });
        }

        public async Task<MergeRequest> GetMergeRequestFromCurrentIssue(GitlabIssue gitlabIssue, GitlabProject project)
        {
            IList<MergeRequest> mergeRequests = 
                await _client.MergeRequests.GetAsync(project.Id);

            return mergeRequests
                .SingleOrDefault(m => m.SourceBranch == $"issue/{gitlabIssue.Id}");
        }

        private async Task<Milestone> GetCurrentSprint(GitlabProject project)
        {
            if (!string.IsNullOrEmpty(project.GroupId))
            {
                IList<Milestone> millestones = await _client.Groups.GetMilestonesAsync(project.GroupId, options => options.State = MilestoneState.Active);

                return millestones
                    .OrderBy(m => m.StartDate)
                    .First();
            }

            if (!string.IsNullOrEmpty(project.Id))
            {
                IList<Milestone> millestones = await _client.Projects.GetMilestonesAsync(project.Id, options => options.State = MilestoneState.Active);

                return millestones
                    .OrderBy(m => m.StartDate)
                    .First();
            }

            return await Task.FromResult(new Milestone());
        }

        private static CreateMergeRequest GetMergeRequestInfo(string sourceBranch, string mrTitle, string issueId)
        {
            CreateMergeRequest createdMergeRequest = new CreateMergeRequest(sourceBranch, "master", mrTitle)
            {
                Description = $"Closes #{issueId}",
                RemoveSourceBranch = true
            };

            return createdMergeRequest;
        }

        public async Task<IList<Issue>> GetIssuesFromCurrentProject(GitlabProject project)
        {
            if (project.Id != null)
            {
                Milestone milestone = await GetCurrentSprint(project);
                IList<Issue> issues = await _client.Issues.GetAsync(project.Id, options =>
                {
                    options.State = IssueState.All;
                    options.MilestoneTitle = milestone.Title;
                });

                return issues;
            }

            IList<Issue> emptyList = new List<Issue>();

            return await Task.FromResult(emptyList);
        }

        public Task<IList<Label>> GetLabelsFromCurrentProject(GitlabProject project)
        {
            return _client.Projects.GetLabelsAsync(project.Id);
        }

        public async Task SetLabelOnCurrentIssue(GitlabIssue issue, GitlabProject project, string labelNameToAdd = null, string labelNameToRemove = null)
        {
            List<string> labels = issue.Labels;

            if (labelNameToAdd != null)
            {
                labels.Add(labelNameToAdd);
            }

            if (labelNameToRemove != null)
            {
                labels.RemoveAt(labels.IndexOf(labelNameToRemove));
            }

            await _client.Issues.UpdateAsync(project.Id, issue.Id, new UpdateIssueRequest
            {
                Labels = labels
            });
        }

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee, GitlabProject project)
        {
            List<int> assignees = issue.Assignees.Select(a => a.Id).ToList();
            assignees.Add(assignee.Id);
            await _client.Issues.UpdateAsync(project.Id, issue.Id, new UpdateIssueRequest
            {
                Assignees = assignees
            });
        }

        public async Task SetAssigneeOnMergeRequest(GitlabIssue gitlabIssue, GitlabProject project)
        {
            MergeRequest mergeRequest = await GetMergeRequestFromCurrentIssue(gitlabIssue, project);
            Assignee assignee = await GetAssigneeFromCurrentUser();

            await _client.MergeRequests.UpdateAsync(project.Id, mergeRequest.Iid, new UpdateMergeRequest
            {
                AssigneeId = assignee.Id
            });
        }

        public async Task SetMilestoneOnMergeRequest(GitlabIssue gitlabIssue, GitlabProject project)
        {
            MergeRequest mergeRequest = await GetMergeRequestFromCurrentIssue(gitlabIssue, project);
            
            await _client.MergeRequests.UpdateAsync(project.Id, mergeRequest.Iid, new UpdateMergeRequest
            {
                MilestoneId = gitlabIssue.Milestone.Id
            });
        }

        public Task<Project> GetProject(string projectId)
        {
            return _client.Projects.GetAsync(projectId);
        }

        public Task<Group> GetGroup(string groupId)
        {
            return _client.Groups.GetAsync(groupId);
        }

        public async Task<Assignee> GetAssigneeFromCurrentUser()
        {
            Session session = await _client.Users.GetCurrentSessionAsync();
            Assignee assignee = new Assignee()
            {
                Id = session.Id,
                Name = session.Name,
                Username = session.Username,
                State = session.State,
                WebUrl = session.WebsiteUrl,
                AvatarUrl = session.AvatarUrl,
                CreatedAt = session.CreatedAt
            };

            return assignee;
        } 
    }
}