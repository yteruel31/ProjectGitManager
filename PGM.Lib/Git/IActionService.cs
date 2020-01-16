using PGM.Lib.Model;

namespace PGM.Lib.Git
{
    public interface IActionService
    {
        void CreateBranchLinkedWithIssue(GitlabIssue issue);

        void ValidateActualBranch();
    }
}