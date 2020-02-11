using LibGit2Sharp;
using PGM.Model;

namespace PGM.Service.Git
{
    public interface IGitRepository
    {
        GitResult DeleteLocalBranch(Branch branch);

        GitResult PushOnOriginBranch(Branch branch, bool force);

        GitResult PushOnOriginMaster();

        GitResult<RebaseStatus> RebaseOntoMaster(Branch actualBranch);

        GitResult<Branch> GetActualBranch(string issueId);

        GitResult<MergeStatus> PullOnRepository();

        GitResult<Branch> CheckoutIssueBranch(string issueId);

        GitResult<Branch> CheckoutMaster();

        void SetupRepository(GitlabProject currentProject);
    }
}