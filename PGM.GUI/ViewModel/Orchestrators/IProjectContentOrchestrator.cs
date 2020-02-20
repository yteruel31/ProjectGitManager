using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public interface IProjectContentOrchestrator
    {
        Task CreateMergeRequestActualBranch(GitlabIssue issue);

        Task ValidateActualBranch(GitlabIssue issue);

        Task CreateNewBranch(GitlabIssue issue);

        Task<List<GitlabIssue>> GetGitlabIssues(GitlabProject project);

        Task TestActualBranch(GitlabIssue issue);

        void SetupRepositoryOnCurrentProject(GitlabProject currentProject);

        Task<bool> MergeRequestFromCurrentIssueHaveConflict(GitlabIssue issue);
    }
}