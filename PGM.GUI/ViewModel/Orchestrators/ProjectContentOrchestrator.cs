using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;
using PGM.Service.Git;
using PGM.Service.Gitlab;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public class ProjectContentOrchestrator : IProjectContentOrchestrator
    {
        private readonly IGitlabService _gitlabService;
        private readonly IGitService _gitService;

        public ProjectContentOrchestrator(
            IGitlabService gitlabService, 
            IGitService gitService)
        {
            _gitlabService = gitlabService;
            _gitService = gitService;
        }

        public void SetupRepositoryOnCurrentProject(GitlabProject currentProject)
        {
            _gitService.SetupRepositoryOnCurrentProject(currentProject);
        }

        public async Task ValidateActualBranch(GitlabIssue issue)
        {
            _gitService.RebaseActualBranchOntoMaster(issue);
            await _gitlabService.ValidateMergeRequest(issue);
            _gitService.DeleteActualBranch(issue);
        }

        public async Task CreateMergeRequestActualBranch(GitlabIssue issue)
        {
            _gitService.CheckoutOnBranch(true);
            await _gitlabService.CreateMergeRequest(issue);
            await _gitlabService.SetMilestoneOnMergeRequest(issue);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, StepType.ToValidate);
        }

        public async Task CreateNewBranch(GitlabIssue issue)
        {
            _gitService.CreateBranchLinkedWithIssue(issue);
            await _gitlabService.SetAssigneeOnCurrentIssue(issue);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, StepType.InProgress);
        }

        public async Task<List<GitlabIssue>> GetGitlabIssues(GitlabProject project)
        {
            return await _gitlabService.GetAllIssuesOfCurrentSprint(project);
        }

        public async Task TestActualBranch(GitlabIssue issue)
        {
            _gitService.CheckoutOnBranch(false, issue);
            await _gitlabService.SetAssigneeOnMergeRequest(issue);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, StepType.Validating);
        }

        public Task<bool> MergeRequestFromCurrentIssueHaveConflict(GitlabIssue issue)
        {
            return _gitlabService.MergeRequestFromCurrentIssueHaveConflict(issue);
        }
     }
}