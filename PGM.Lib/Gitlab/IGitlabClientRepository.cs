using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;

namespace PGM.Lib.Gitlab
{
    public interface IGitlabClientRepository
    {
        void PostMergeRequest(string branche, string mrTitle);

        Task<IList<Issue>> GetIssuesFromCurrentProject();

        Task<IList<Label>> GetLabelsFromCurrentProject();
    }
}