using LibGit2Sharp;

namespace PGM.Service.Git
{
    public interface IGitRepository
    {
        GitResult DeleteLocalBranch(Branch branch);

        GitResult PushOnOriginBranch(Branch branch);

        GitResult PushOnOriginMaster();

        GitResult<RebaseStatus> RebaseOntoMaster(Branch actualBranch);

        GitResult<MergeStatus> PullOnRepository();

        GitResult<Branch> CheckoutIssueBranch(string issueId);

        GitResult<Branch> CheckoutMaster();
    }
}