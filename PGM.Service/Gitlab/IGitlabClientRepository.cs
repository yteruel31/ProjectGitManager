using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Groups.Responses;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Responses;
using GitLabApiClient.Models.Projects.Responses;
using GitLabApiClient.Models.Users.Responses;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public interface IGitlabClientRepository
    {
        Task PostMergeRequest(string branch, string mrTitle, GitlabIssue issue, GitlabProject currentProject);

        Task<IList<Issue>> GetIssuesFromCurrentProject(GitlabProject project);

        Task<IList<Label>> GetLabelsFromCurrentProject(GitlabProject project);

        Task<MergeRequest> GetMergeRequestFromCurrentIssue(GitlabIssue issue, GitlabProject project);

        Task<Assignee> GetAssigneeFromCurrentUser();

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee, GitlabProject project);

        Task ValidateMergeRequest(GitlabIssue issue, GitlabProject currentProject);

        Task<Project> GetProject(string projectId);

        Task<Group> GetGroup(string groupId);

        Task SetLabelOnCurrentIssue(GitlabIssue issue, GitlabProject project, string labelNameToAdd = null,
            string labelNameToRemove = null);

        Task SetAssigneeOnMergeRequest(GitlabIssue gitlabIssue, GitlabProject project);

        Task SetMilestoneOnMergeRequest(GitlabIssue gitlabIssue, GitlabProject project);

        Task<IList<Group>> GetGroupsFromCurrentUser();

        Task<IList<Project>> GetProjectsFromCurrentUser();

        Task<Session> GetCurrentUser();

        Task<IList<Project>> GetProjectsByGroupId(int groupId);
    }
}