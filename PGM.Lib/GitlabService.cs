using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using PGM.GUI.ViewModel;
using PGM.Lib.Model;

namespace PGM.Lib
{
    public class GitlabService
    {
        private readonly GitLabClient _client;
        private readonly ISettings _settings;

        public GitlabService(ISettings settings)
        {
            _settings = settings;
            _client = new GitLabClient("https://gitlab.com/", _settings.GitApiKey);
        }

        public async Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint()
        {
            IList<Issue> issueResult;
            try
            {
                issueResult = await _client.Issues.GetAsync(_settings.ProjectId);
            }
            catch (GitLabException)
            {
                return new List<GitlabIssue>();
            }
            List<GitlabIssue> gitlabIssues = new List<GitlabIssue>();
            foreach (Issue issue in issueResult)
            {
                GitlabIssue gitlabIssue = GetGitlabIssue(issue);

                if (gitlabIssue.Sprint.Id != issue.Milestone.Id)
                {
                    continue;
                }
                
                gitlabIssues.Add(gitlabIssue);
            }

            return gitlabIssues;
        }

        public void PostMergeRequest(string branche, string mrTitle)
        {
            _client.MergeRequests.CreateAsync(_settings.ProjectId, GetMergeRequestInfo(branche, mrTitle));
        }

        private GitlabIssue GetGitlabIssue(Issue issue)
        {
            return new GitlabIssue()
            {
                Id = issue.Id,
                Description = issue.Description,
                Sprint = new Sprint()
                {
                    Id = issue.Milestone.Id,
                    Title = issue.Milestone.Title
                }
            };
        }

        private CreateMergeRequest GetMergeRequestInfo(string sourceBranch, string mrTitle)
        {
            return new CreateMergeRequest(sourceBranch, "master", mrTitle);
        }
    }
}