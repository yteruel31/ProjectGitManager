using LibGit2Sharp;
using PGM.Model;

namespace PGM.Service.Git
{
    public class GitService : IGitService
    {
        private readonly IGitRepository _gitRepository;

        public GitService(IGitRepository gitRepository)
        {
            _gitRepository = gitRepository;
        }

        public void SetupRepositoryOnCurrentProject(GitlabProject currentProject)
        {
            _gitRepository.SetupRepository(currentProject);
        }

        public void CreateBranchLinkedWithIssue(GitlabIssue issue)
        {
            _gitRepository.CheckoutMaster();
            _gitRepository.PullOnRepository();
            GitResult<Branch> result = _gitRepository.CheckoutIssueBranch(issue.Id.ToString());
            
            if (result.HasSucceeded)
            {
                _gitRepository.PushOnOriginBranch(result.Response, false);
            }
        }

        public void RebaseActualBranchOntoMaster(GitlabIssue issue)
        {
            Branch branch = _gitRepository.GetActualBranch(issue.Id.ToString()).Response;
            CheckoutOnBranch(true);
            CheckoutOnBranch(false, issue);
            _gitRepository.RebaseOntoMaster(branch);
            _gitRepository.PushOnOriginBranch(branch, true);
        }

        public void DeleteActualBranch(GitlabIssue issue)
        {
            Branch branch = _gitRepository.GetActualBranch(issue.Id.ToString()).Response;
            _gitRepository.DeleteLocalBranch(branch);
        }

        public void CheckoutOnBranch(bool isMasterBranch, GitlabIssue issue = null)
        {
            if (isMasterBranch)
            {
                _gitRepository.CheckoutMaster();
                _gitRepository.PullOnRepository();
            }
            else
            {
                if (issue == null)
                {
                    return;
                }

                GitResult<Branch> result = _gitRepository.CheckoutIssueBranch(issue.Id.ToString());

                if (!result.HasSucceeded)
                {
                    return;
                }

                if (result.Response.TrackingDetails.AheadBy > 0)
                {
                    _gitRepository.PullOnRepository();
                }
            }
        }
    }
}