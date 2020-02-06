using PGM.Model;
using PGM.Service.Gitlab;

namespace PGM.Service.Git
{
    public class GitService : IGitService
    {
        private readonly PGMSetting _settings;
        private readonly IGitRepository _gitRepository;
        private readonly IGitlabService _gitlabService;

        public GitService(PGMSetting settings)
        {
            _settings = settings;
            _gitRepository = new GitRepository(settings);
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