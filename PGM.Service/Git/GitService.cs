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

        public void SetupRepositoryOnCurrentProject(string repositoryPath)
        {
            _gitRepository.SetupRepository(repositoryPath);
        }

        public void CreateBranchLinkedWithIssue(GitlabIssue issue)
        {
            _gitRepository.CheckoutMaster();
            _gitRepository.PullOnRepository();
            _gitRepository.CheckoutIssueBranch(issue.Id.ToString());
        }

        public void RebaseActualBranchOntoMaster(GitlabIssue issue)
        {
            Branch branch = _gitRepository.GetActualBranch(issue.Id.ToString()).Response;
            CheckoutOnBranch(true);
            CheckoutOnBranch(false, issue);
            _gitRepository.RebaseOntoMaster(branch);
            _gitRepository.PushOnOriginBranch(branch, true);
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

                _gitRepository.CheckoutIssueBranch(issue.Id.ToString());
                _gitRepository.PullOnRepository();
            }
        }
    }
}