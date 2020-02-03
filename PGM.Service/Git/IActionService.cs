using PGM.Model;

namespace PGM.Service.Git
{
    public interface IActionService
    {
        void CreateBranchLinkedWithIssue(GitlabIssue issue);

        void ValidateActualBranch();
    }
}