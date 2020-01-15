using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.Milestones.Responses;
using GitLabApiClient.Models.Projects.Responses;
using PGM.Lib.Model;

namespace PGM.Lib
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
    }
}