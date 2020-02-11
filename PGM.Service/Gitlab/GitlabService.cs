using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Responses;
using GitLabApiClient.Models.Projects.Responses;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public class GitlabService : IGitlabService
    {
        private readonly IGitlabClientRepository _gitlabClientRepository;

        public GitlabService(IGitlabClientRepository gitlabClientRepository)
        {
            _gitlabClientRepository = gitlabClientRepository;
        }

        public async Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint(GitlabProject project)
        {
            IList<Issue> issueResult;

            try
            {
                issueResult = await _gitlabClientRepository.GetIssuesFromCurrentProject(project);
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
                    GitlabIssue gitlabIssue = GetGitlabIssue(issue, project);
                    SetStepType(gitlabIssue);
                    if (gitlabIssue.GitlabMilestone.Id != issue.Milestone.Id)
                    {
                        continue;
                    }

                    gitlabIssues.Add(gitlabIssue);
                }
            }

            return gitlabIssues;
        }

        private void SetStepType(GitlabIssue gitlabIssue)
        {
            if (gitlabIssue.IsClosed)
            {
                gitlabIssue.StepType = StepType.Done;
            } 
            else if (gitlabIssue.Labels.Any(l => l.Equals("En cours d'implémentation")))
            {
                gitlabIssue.StepType = StepType.InProgress;
            }
            else if (gitlabIssue.Labels.Any(l=> l.Equals("À Valider")))
            {
                gitlabIssue.StepType = StepType.ToValidate;
            }
            else if(gitlabIssue.Labels.Any(l => l.Equals("En cours de validation")))
            {
                gitlabIssue.StepType = StepType.Validating;
            }
        }

        public async Task AssignCorrectLabelRelatedToCurrentIssue(GitlabIssue issue, GitlabProject project, StepType stepType)
        {
            switch (stepType)
            {
                case StepType.InProgress:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, project, "En cours d'implémentation");
                    break;
                case StepType.ToValidate:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, project, "À Valider", "En cours d'implémentation");
                    break;
                case StepType.Validating:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, project, "En cours de validation", "À Valider");
                    break;
                case StepType.Done:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, project, null, "En cours de validation");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private GitlabIssue GetGitlabIssue(Issue issue, GitlabProject currentProject)
        {
            return new GitlabIssue()
            {
                Id = issue.Iid,
                Title = issue.Title,
                Description = issue.Description,
                IsClosed = issue.State == IssueState.Closed,
                GitlabMilestone = new GitlabMilestone()
                {
                    Id = issue.Milestone.Id,
                    Title = issue.Milestone.Title
                },
                Assignees = GetGitlabAssignees(issue.Assignees),
                Labels = issue.Labels,
                StepType = StepType.Backlog,
                WebUrl = issue.WebUrl
            };
        }

        private GitlabProject GetGitlabProject(Project project)
        {
            return new GitlabProject
            {
                Id = project.Id.ToString(),
                Name = project.Name
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

        public async Task<bool> MergeRequestFromCurrentIssueHaveConflict(GitlabIssue gitlabIssue, GitlabProject currentProject)
        {
            if (currentProject.Id == null)
            {
                return await Task.FromResult(false);
            }

            MergeRequest mergeRequest = await _gitlabClientRepository.GetMergeRequestFromCurrentIssue(gitlabIssue, currentProject);
            return mergeRequest.Status == MergeStatus.CannotBeMerged;
        }

        public Task CreateMergeRequest(GitlabIssue currentIssue, GitlabProject currentProject)
        {
            string mrTitle = $"Issue/{currentIssue.Id} - {currentIssue.Title}";
            return _gitlabClientRepository.PostMergeRequest($"issue/{currentIssue.Id}", mrTitle, currentIssue, currentProject);
        }

        private IEnumerable<Label> GetLabelsFromCurrentIssue(IList<Label> labelsResult, Issue currentIssue)
        {
            return labelsResult.Where(l => currentIssue.Labels.Contains(l.Name));
        }

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue, GitlabProject project)
        {
            Assignee assignee = await _gitlabClientRepository.GetAssigneeFromCurrentUser();
            await _gitlabClientRepository.SetAssigneeOnCurrentIssue(issue, assignee, project);
        }

        public Task SetAssigneeOnMergeRequest(GitlabIssue issue, GitlabProject project)
        {
            return _gitlabClientRepository.SetAssigneeOnMergeRequest(issue, project);
        }

        public Task ValidateMergeRequest(GitlabIssue issue, GitlabProject currentProject)
        {
            return _gitlabClientRepository.ValidateMergeRequest(issue, currentProject);
        }

        public async Task<GitlabProject> GetProject(string projectId)
        {
            Project project = await _gitlabClientRepository.GetProject(projectId);
            return GetGitlabProject(project);
        }

        public Task<bool> GroupExist(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return Task.FromResult(false);
            }

            Task task = _gitlabClientRepository.GetGroup(groupId);

            return Task.FromResult(!task.IsCanceled);
        }

        public Task<bool> ProjectExist(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return Task.FromResult(false);
            }

            Task task = _gitlabClientRepository.GetProject(projectId);

            return Task.FromResult(!task.IsCanceled);
        }
    }
}