using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabApiClient.Models.Issues.Responses;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public interface IGitlabService
    {
        Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint(GitlabProject project);

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue);

        Task CreateMergeRequest(GitlabIssue issue);

        Task ValidateMergeRequest(GitlabIssue issue);

        Task<bool> ProjectExist(string projectId);

        Task<GitlabProject> GetProject(string projectId);

        Task<bool> GroupExist(string groupId);

        Task AssignCorrectLabelRelatedToCurrentIssue(GitlabIssue issue, StepType stepType);

        Task SetAssigneeOnMergeRequest(GitlabIssue issue);

        Task SetMilestoneOnMergeRequest(GitlabIssue issue);

        Task<bool> MergeRequestFromCurrentIssueHaveConflict(GitlabIssue gitlabIssue);

        Task<IEnumerable<GitlabProject>> GetProjectsFromCurrentUser();
    }
}