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
        private readonly PGMSetting _settings;

        public GitlabClientRepository(PGMSetting settings)
        {
            _settings = settings;
            _client = new GitLabClient("https://gitlab.com/", settings.GitLabApiKey);
        }
        
        public async Task PostMergeRequest(string branch, string mrTitle, GitlabIssue issue)
        {
            Issue currentIssue = await GetIssue(issue);
            await _client.MergeRequests.CreateAsync(_settings.CurrentProject.Id, GetMergeRequestInfo(branch, mrTitle, currentIssue));
        }

        public async Task ValidateMergeRequest(GitlabIssue issue)
        {
            Issue currentIssue = await GetIssue(issue);
            MergeRequest mergeRequest = await GetMergeRequestFromCurrentIssue(currentIssue);
            await _client.MergeRequests.AcceptAsync(_settings.CurrentProject.Id, mergeRequest.Iid, new AcceptMergeRequest
            {
                RemoveSourceBranch = true,
                MergeWhenPipelineSucceeds = false,
                MergeCommitMessage = $"[{currentIssue.Iid}] - {currentIssue.Title}"
            });
        }

        private async Task<MergeRequest> GetMergeRequestFromCurrentIssue(Issue issue)
        {
            IList<MergeRequest> mergeRequests = 
                await _client.MergeRequests.GetAsync(_settings.CurrentProject.Id);

            return mergeRequests
                .SingleOrDefault(m => m.SourceBranch == $"issue/{issue.Iid}");
        }

        public async Task<Milestone> GetCurrentSprint()
        {
            IList<Milestone> millestones = await _client.Groups.GetMilestonesAsync(_settings.CurrentProject.Id);

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

        public Task<IList<Issue>> GetIssuesFromCurrentProject()
        {
            return _client.Issues.GetAsync(_settings.CurrentProject.Id);
        }

        public Task<IList<Label>> GetLabelsFromCurrentProject()
        {
            return _client.Projects.GetLabelsAsync(_settings.CurrentProject.Id);
        }

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee)
        {
            Issue currentIssue = await GetIssue(issue);
            List<int> assignees = currentIssue.Assignees.Select(a => a.Id).ToList();
            assignees.Add(assignee.Id);
            await _client.Issues.UpdateAsync(_settings.CurrentProject.Id, issue.Id, new UpdateIssueRequest
            {
                Assignees = assignees
            });
        }

        private Task<Issue> GetIssue(GitlabIssue issue)
        {
            return _client.Issues.GetAsync(_settings.CurrentProject.Id, issue.Id);
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