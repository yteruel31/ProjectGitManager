using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Groups.Responses;
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
            catch (Exception)
            {
                return new List<GitlabIssue>();
            }

            List<GitlabIssue> gitlabIssues = new List<GitlabIssue>();

            foreach (Issue issue in issueResult)
            {
                if (issue.Milestone != null)
                {
                    GitlabIssue gitlabIssue = GetGitlabIssue(issue);
                    SetStepType(gitlabIssue);
                    if (gitlabIssue.Milestone.Id != issue.Milestone.Id)
                    {
                        continue;
                    }

                    gitlabIssues.Add(gitlabIssue);
                }
            }

            return gitlabIssues;
        }

        private static void SetStepType(GitlabIssue gitlabIssue)
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

        public async Task AssignCorrectLabelRelatedToCurrentIssue(GitlabIssue issue, StepType stepType)
        {
            switch (stepType)
            {
                case StepType.InProgress:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, issue.CurrentProject, "En cours d'implémentation");
                    break;
                case StepType.ToValidate:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, issue.CurrentProject, "À Valider", "En cours d'implémentation");
                    break;
                case StepType.Validating:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, issue.CurrentProject, "En cours de validation", "À Valider");
                    break;
                case StepType.Done:
                    await _gitlabClientRepository.SetLabelOnCurrentIssue(issue, issue.CurrentProject, null, "En cours de validation");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private GitlabIssue GetGitlabIssue(Issue issue)
        {
            return new GitlabIssue()
            {
                Id = issue.Iid,
                Title = issue.Title,
                Description = issue.Description,
                IsClosed = issue.State == IssueState.Closed,
                Milestone = new GitlabMilestone()
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

        private static GitlabProject GetGitlabProject(Project project)
        {
            return new GitlabProject
            {
                Id = project.Id.ToString(),
                Name = project.Name
            };
        }

        private static List<GitlabAssignee> GetGitlabAssignees(List<Assignee> assignees)
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

        public async Task<bool> MergeRequestFromCurrentIssueHaveConflict(GitlabIssue gitlabIssue)
        {
            if (gitlabIssue.CurrentProject.Id == null)
            {
                return await Task.FromResult(false);
            }

            MergeRequest mergeRequest = await _gitlabClientRepository.GetMergeRequestFromCurrentIssue(gitlabIssue, gitlabIssue.CurrentProject);
            return mergeRequest.Status == MergeStatus.CannotBeMerged;
        }

        public Task CreateMergeRequest(GitlabIssue issue)
        {
            string mrTitle = $"Issue/{issue.Id} - {issue.Title}";
            return _gitlabClientRepository.PostMergeRequest($"issue/{issue.Id}", mrTitle, issue, issue.CurrentProject);
        }

        private IEnumerable<Label> GetLabelsFromCurrentIssue(IList<Label> labelsResult, Issue currentIssue)
        {
            return labelsResult.Where(l => currentIssue.Labels.Contains(l.Name));
        }

        public async Task SetAssigneeOnCurrentIssue(GitlabIssue issue)
        {
            Assignee assignee = await _gitlabClientRepository.GetAssigneeFromCurrentUser();
            await _gitlabClientRepository.SetAssigneeOnCurrentIssue(issue, assignee, issue.CurrentProject);
        }

        public Task SetAssigneeOnMergeRequest(GitlabIssue issue)
        {
            return _gitlabClientRepository.SetAssigneeOnMergeRequest(issue, issue.CurrentProject);
        }

        public Task SetMilestoneOnMergeRequest(GitlabIssue issue)
        {
            return _gitlabClientRepository.SetMilestoneOnMergeRequest(issue, issue.CurrentProject);
        }

        public Task ValidateMergeRequest(GitlabIssue issue)
        {
            return _gitlabClientRepository.ValidateMergeRequest(issue, issue.CurrentProject);
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

        public async Task<IEnumerable<GitlabProject>> GetProjectsFromCurrentUser()
        {
            IList<Group> groupsFromCurrentUser = await _gitlabClientRepository.GetGroupsFromCurrentUser();
            IList<Project> projectsFromCurrentUser = await _gitlabClientRepository.GetProjectsFromCurrentUser();
            List<GitlabProject> projects = new List<GitlabProject>();
            
            foreach (Group group in groupsFromCurrentUser)
            {
                IList<Project> projectsByGroup = await _gitlabClientRepository.GetProjectsByGroupId(group.Id);

                foreach (Project groupProject in projectsByGroup)
                {
                    GitlabProject project =  new GitlabProject
                    {
                        GroupId = group.Id.ToString(),
                        GroupName = group.Name,
                        Id = groupProject.Id.ToString(),
                        Name = groupProject.Name
                    };
                    projects.Add(project);
                }
            }

            AddProjectsFromCurrentUser(projectsFromCurrentUser, projects);

            return projects;
        }

        private static void AddProjectsFromCurrentUser(IList<Project> projectsFromCurrentUser, List<GitlabProject> projects)
        {
            foreach (Project project in projectsFromCurrentUser.Where(project => project.Namespace.Kind == "user"))
            {
                GitlabProject gitlabProject = new GitlabProject
                {
                    GroupName = project.Owner.Name,
                    Id = project.Id.ToString(),
                    Name = project.Name
                };
                projects.Add(gitlabProject);
            }
        }
    }
}