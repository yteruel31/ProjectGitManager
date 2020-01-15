using PGM.Lib.Model.Issues;

namespace PGM.Lib.Git
{
    public interface IActionService
    {
        void CreateBranchLinkedWithIssue(GitlabIssue issue);

        void ValidateActualBranch();
    }
}