using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabApiClient.Models;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public interface IGitlabClientRepository
    {
        void PostMergeRequest(string branche, string mrTitle);

        Task<IList<Issue>> GetIssuesFromCurrentProject();

        Task<IList<Label>> GetLabelsFromCurrentProject();

        Task<Assignee> GetAssigneeFromCurrentUser();

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue, Assignee assignee);
    }
}