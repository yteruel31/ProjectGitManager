using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabApiClient.Models.Projects.Responses;
using PGM.Lib.Model;
using PGM.Lib.Model.Issues;

namespace PGM.Lib
{
    public class GitlabService
    {
        private readonly GitLabClient _client;
        private readonly IPGMSettings _settings;

        public GitlabService(IPGMSettings settings)
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

                if (gitlabIssue.GitlabMilestone.Id != issue.Milestone.Id)
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
                GitlabMilestone = new GitlabMilestone()
                {
                    Id = issue.Milestone.Id,
                    Title = issue.Milestone.Title
                }
            };
        }

        private GitlabLabel GetGitlabLabel(Label label)
        {
            return new GitlabLabel()
            {
                Id = label.Id,
                Name = label.Name,
                Color = label.Color,
                Description = label.Description
            };
        }

        private async Task<List<GitlabLabel>> GetAllRelatedLabelsFromCurrentIssue(Issue currentIssue)
        {
            IList<Label> labelsResult;

            try
            {
                labelsResult = await _client.Projects.GetLabelsAsync(_settings.ProjectId);
            }
            catch (GitLabException)
            {
                return new List<GitlabLabel>();
            }

            IList<Label> labelsFromCurrentIssue = GetLabelsFromCurrentIssue(labelsResult, currentIssue).ToList();

            List<GitlabLabel> gitlabLabels = new List<GitlabLabel>();

            foreach (Label labelFromCurrentIssue in labelsFromCurrentIssue)
            {
                GitlabLabel gitlabLabel = GetGitlabLabel(labelFromCurrentIssue);

                gitlabLabels.Add(gitlabLabel);
            }

            return gitlabLabels;
        }

        private IEnumerable<Label> GetLabelsFromCurrentIssue(IList<Label> labelsResult, Issue currentIssue)
        {
            return labelsResult.Where(l => currentIssue.Labels.Contains(l.Name));
        }

        private CreateMergeRequest GetMergeRequestInfo(string sourceBranch, string mrTitle)
        {
            return new CreateMergeRequest(sourceBranch, "master", mrTitle);
        }
    }
}