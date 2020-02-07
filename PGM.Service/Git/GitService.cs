using PGM.Model;
using PGM.Service.Gitlab;

namespace PGM.Service.Git
{
    public class GitService : IGitService
    {
        private readonly IGitRepository _gitRepository;

        public GitService(IGitRepository gitRepository)
        {
            _gitRepository = gitRepository;
        }

        public void CreateBranchLinkedWithIssue(GitlabIssue issue)
        {
            _gitRepository.CheckoutIssueBranch(issue.Id.ToString());
        }

        public void CheckoutOnBranch(bool isMasterBranch, GitlabIssue issue = null)
        {
            if (isMasterBranch)
            {
                _gitRepository.CheckoutMaster();
            }
            else
            {
                if (issue != null)
                {
                    _gitRepository.CheckoutIssueBranch(issue.Id.ToString());
                }
            }
        }
    }
}