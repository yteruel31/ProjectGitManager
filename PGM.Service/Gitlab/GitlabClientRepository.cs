using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.MergeRequests.Responses;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;
using GitLabApiClient.Models.Users.Responses;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public class GitlabClientRepository : IGitlabClientRepository
    {
        private readonly GitLabClient _client;
        private PGMSetting Settings => _pgmSettingManagerService.CurrentSettings;
        private readonly IPgmSettingManagerService _pgmSettingManagerService;
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
            Issue currentIssue = await GetIssue(currentProject, issue);
            await _client.MergeRequests.CreateAsync(currentProject.Id, GetMergeRequestInfo(branch, mrTitle, currentIssue));
        }

        public async Task ValidateMergeRequest(GitlabIssue issue, GitlabProject currentProject)
        {
            Issue currentIssue = await GetIssue(currentProject ,issue);
            MergeRequest mergeRequest = await GetMergeRequestFromCurrentIssue(currentIssue, currentProject);
            await _client.MergeRequests.AcceptAsync(currentProject.Id, mergeRequest.Iid, new AcceptMergeRequest
            {
                RemoveSourceBranch = true,
                MergeWhenPipelineSucceeds = false,
                MergeCommitMessage = $"[{currentIssue.Iid}] - {currentIssue.Title}"
            });
        }

        private async Task<MergeRequest> GetMergeRequestFromCurrentIssue(Issue issue, GitlabProject project)
        {
            IList<MergeRequest> mergeRequests = 
                await _client.MergeRequests.GetAsync(project.Id);

            return mergeRequests
                .SingleOrDefault(m => m.SourceBranch == $"issue/{issue.Iid}");
        }

        public async Task<Milestone> GetCurrentSprint()
        {
            IList<Milestone> millestones = await _client.Groups.GetMilestonesAsync(Settings.CurrentGitlabProject.Id);

            return millestones
                .OrderBy(m => Convert.ToDateTime(m.StartDate))
                .First(m => m.State == MilestoneState.Active);
        }

        private CreateMergeRequest GetMergeRequestInfo(string sourceBranch, string mrTitle, Issue issue)
        {
            CreateMergeRequest createdMergeRequest = new CreateMergeRequest(sourceBranch, "master", mrTitle);
            createdMergeRequest.Description = $"Closes #{issue.Iid}";
            return createdMergeRequest;
        }

        public Task<IList<Issue>> GetIssuesFromCurrentProject(GitlabProject project)
        {
            if (project.Id != null)
            {
                return _client.Issues.GetAsync(project.Id);
            }

            IList<Issue> emptyList = new List<Issue>();

            return Task.FromResult(emptyList);
        }

        public Task<IList<Label>> GetLabelsFromCurrentProject(GitlabProject project)
        {
            return _client.Projects.GetLabelsAsync(project.Id);
        }

        public async Task SetLabelOnCurrentIssue(GitlabIssue issue, GitlabProject project, string labelName)
        {
            Issue currentIssue = await GetIssue(project, issue);
            List<string> labels = currentIssue.Labels;
            labels.Add(labelName);
            await _client.Issues.UpdateAsync(project.Id, currentIssue.Iid, new UpdateIssueRequest
            {
                Labels = labels
            });
        }

        public async Task RemoveLabelOnCurrentIssue(GitlabIssue issue, GitlabProject project, string labelName)
        {
            Issue currentIssue = await GetIssue(project, issue);
            List<string> labels = currentIssue.Labels;
            labels.Remove(labelName);
            await _client.Issues.UpdateAsync(project.Id, currentIssue.Iid, new UpdateIssueRequest
            {
                Labels = labels
            });
        }

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee, GitlabProject project)
        {
            Issue currentIssue = await GetIssue(project, issue);
            List<int> assignees = currentIssue.Assignees.Select(a => a.Id).ToList();
            assignees.Add(assignee.Id);
            await _client.Issues.UpdateAsync(project.Id, issue.Id, new UpdateIssueRequest
            {
                Assignees = assignees
            });
        }

        public async Task SetAssigneeOnMergeRequest(GitlabIssue gitlabIssue, GitlabProject project)
        {
            Issue issue = await GetIssue(project, gitlabIssue);
            MergeRequest mergeRequest = await GetMergeRequestFromCurrentIssue(issue, project);
            Assignee assignee = await GetAssigneeFromCurrentUser();

            await _client.MergeRequests.UpdateAsync(project.Id, mergeRequest.Id, new UpdateMergeRequest
            {
                AssigneeId = assignee.Id
            });
        }

        private Task<Issue> GetIssue(GitlabProject project, GitlabIssue issue)
        {
            return _client.Issues.GetAsync(project.Id, issue.Id);
        }

        public Task<Project> GetProject(string projectId)
        {
            return _client.Projects.GetAsync(projectId);
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