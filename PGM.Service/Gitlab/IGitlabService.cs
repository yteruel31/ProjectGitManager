using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public interface IGitlabService
    {
        Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint(GitlabProject project);

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue, GitlabProject project);

        Task CreateMergeRequest(GitlabIssue currentIssue, GitlabProject currentProject);

        Task ValidateMergeRequest(GitlabIssue issue, GitlabProject currentProject);

        Task<bool> ProjectExist(string projectId);

        Task<GitlabProject> GetProject(string projectId);

        Task AssignCorrectLabelRelatedToCurrentIssue(GitlabIssue issue, GitlabProject project, StepType stepType);

        Task SetAssigneeOnMergeRequest(GitlabIssue issue, GitlabProject project);
    }
}