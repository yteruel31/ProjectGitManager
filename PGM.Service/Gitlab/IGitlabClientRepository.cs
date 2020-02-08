using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public interface IGitlabClientRepository
    {
        Task PostMergeRequest(string branch, string mrTitle, GitlabIssue issue, GitlabProject currentProject);

        Task<IList<Issue>> GetIssuesFromCurrentProject(GitlabProject project);

        Task<IList<Label>> GetLabelsFromCurrentProject(GitlabProject project);

        Task<Assignee> GetAssigneeFromCurrentUser();

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee, GitlabProject project);

        Task ValidateMergeRequest(GitlabIssue issue, GitlabProject currentProject);

        Task<Project> GetProject(string projectId);

        Task RemoveLabelOnCurrentIssue(GitlabIssue issue, GitlabProject project, string labelName);

        Task SetLabelOnCurrentIssue(GitlabIssue issue, GitlabProject project, string labelName);

        Task SetAssigneeOnMergeRequest(GitlabIssue gitlabIssue, GitlabProject project);
    }
}