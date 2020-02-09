using System.Collections.Generic;
using System.Threading.Tasks;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public interface IProjectContentOrchestrator
    {
        Task CreateMergeRequestActualBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo);

        Task ValidateActualBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo);

        Task CreateNewBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo);

        Task<List<GitlabIssueVO>> GetGitlabIssue(ProjectVO project);

        Task TestActualBranch(GitlabIssueVO issueVo, ProjectVO projectVo);

        void SetupRepositoryOnCurrentProject(string repositoryPath);

        Task<bool> MergeRequestFromCurrentIssueHaveConflict(GitlabIssueVO issueVo, ProjectVO projectVo);
    }
}