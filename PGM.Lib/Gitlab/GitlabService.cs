using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;
using PGM.Lib.Model;
using PGM.Lib.Utilities;

namespace PGM.Lib.Gitlab
{
    public class GitlabService : IGitlabService
    {
        private readonly IGitlabClientRepository _gitlabClientRepository;

        public GitlabService(IPGMSettings settings)
        {
            _gitlabClientRepository = new GitlabClientRepository(settings);
        }

        public async Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint()
        {
            IList<Issue> issueResult;

            try
            {
                issueResult = await _gitlabClientRepository.GetIssuesFromCurrentProject();
            }
            catch (GitLabException)
            {
                return new List<GitlabIssue>();
            }

            List<GitlabIssue> gitlabIssues = new List<GitlabIssue>();

            foreach (Issue issue in issueResult)
            {
                if (issue.Milestone != null)
                {
                    GitlabIssue gitlabIssue = GetGitlabIssue(issue);

                    if (gitlabIssue.GitlabMilestone.Id != issue.Milestone.Id)
                    {
                        continue;
                    }

                    gitlabIssues.Add(gitlabIssue);
                }
            }

            return gitlabIssues;
        }

        private GitlabIssue GetGitlabIssue(Issue issue)
        {
            return new GitlabIssue()
            {
                Id = issue.Id,
                Title = issue.Title,
                Description = issue.Description,
                GitlabMilestone = new GitlabMilestone()
                {
                    Id = issue.Milestone.Id,
                    Title = issue.Milestone.Title
                },
                Assignees = GetGitlabAssignees(issue.Assignees)
            };
        }

        private List<GitlabAssignee> GetGitlabAssignees(List<Assignee> assignees)
        {
            return assignees.Select(assignee => new GitlabAssignee
                {
                    Id = assignee.Id,
                    State = assignee.State,
                    Name = assignee.Name,
                    Username = assignee.Username,
                    CreatedAt = assignee.CreatedAt,
                    AvatarUrl = assignee.AvatarUrl
                }).ToList();
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
                labelsResult = await _gitlabClientRepository.GetLabelsFromCurrentProject();
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

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue)
        {
            Assignee assignee = await _gitlabClientRepository.GetAssigneeFromCurrentUser();
            issue.Assignees.Add(assignee.ToGitLabAssignee());
        }
    }

    public interface IGitlabService
    {
        Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint();

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue);
    }
}