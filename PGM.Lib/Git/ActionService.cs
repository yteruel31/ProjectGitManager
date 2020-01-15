using PGM.Lib.Gitlab;
using PGM.Lib.Model;
using PGM.Lib.Model.Issues;

namespace PGM.Lib.Git
{
    public class ActionService : IActionService
    {
        private readonly IPGMSettings _settings;
        private readonly IGitRepository _gitRepository;
        private readonly IGitlabService _gitlabService;

        public ActionService(IPGMSettings settings)
        {
            _settings = settings;
            _gitRepository = new GitRepository(settings);
        }

        public void CreateBranchLinkedWithIssue(GitlabIssue issue)
        {
            _gitRepository.CheckoutIssueBranch(issue.Id.ToString());
        }

        public void ValidateActualBranch()
        {
            
        }
    }
}