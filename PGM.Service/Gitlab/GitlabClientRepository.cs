﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Issues.Requests;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;
using GitLabApiClient.Models.Users.Responses;
using PGM.Model;
using PGM.Service.Utilities;

namespace PGM.Service.Gitlab
{
    public class GitlabClientRepository : IGitlabClientRepository
    {
        private readonly GitLabClient _client;
        private readonly IPGMSettings _settings;

        public GitlabClientRepository(IPGMSettings settings)
        {
            _settings = settings;
            _client = new GitLabClient("https://gitlab.com/", settings.GitApiKey);
        }
        
        public void PostMergeRequest(string branche, string mrTitle)
        {
            _client.MergeRequests.CreateAsync(_settings.ProjectId, GetMergeRequestInfo(branche, mrTitle));
        }

        public async Task<Milestone> GetCurrentSprint()
        {
            IList<Milestone> millestones = await _client.Groups.GetMilestonesAsync(_settings.ProjectId);

            return millestones
                .OrderBy(m => Convert.ToDateTime(m.StartDate))
                .First(m => m.State == MilestoneState.Active);
        }

        private CreateMergeRequest GetMergeRequestInfo(string sourceBranch, string mrTitle)
        {
            return new CreateMergeRequest(sourceBranch, "master", mrTitle);
        }

        public Task<IList<Issue>> GetIssuesFromCurrentProject()
        {
            return _client.Issues.GetAsync(_settings.ProjectId);
        }

        public Task<IList<Label>> GetLabelsFromCurrentProject()
        {
            return _client.Projects.GetLabelsAsync(_settings.ProjectId);
        }

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee)
        {
            Issue currentIssue = await GetIssue(issue);
            List<int> assignees = currentIssue.Assignees.Select(a => a.Id).ToList();
            assignees.Add(assignee.Id);
            await _client.Issues.UpdateAsync(_settings.ProjectId, issue.Id, new UpdateIssueRequest
            {
                Assignees = assignees
            });
        }

        private Task<Issue> GetIssue(GitlabIssue issue)
        {
            return _client.Issues.GetAsync(_settings.ProjectId, issue.Id);
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