using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;

namespace PGM.Service.Gitlab
{
    public interface IGitlabService
    {
        Task<List<GitlabIssue>> GetAllIssuesOfCurrentSprint();

        Task SetAssigneeOnCurrentIssue(GitlabIssue issue);

        Task CreateMergeRequest(GitlabIssue currentIssue);

        Task ValidateMergeRequest(GitlabIssue issue);
    }
}