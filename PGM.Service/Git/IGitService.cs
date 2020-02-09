using PGM.Model;

namespace PGM.Service.Git
{
    public interface IGitService
    {
        void CreateBranchLinkedWithIssue(GitlabIssue issue);

        void CheckoutOnBranch(bool isMasterBranch, GitlabIssue issue = null);

        void SetupRepositoryOnCurrentProject(string repositoryPath);

        void RebaseActualBranchOntoMaster(GitlabIssue issue);
    }
}