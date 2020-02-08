using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PGM.GUI.AutoMapper;
using PGM.Model;
using PGM.Service.Git;
using PGM.Service.Gitlab;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public class ProjectContentOrchestrator : IProjectContentOrchestrator
    {
        private IGitlabService _gitlabService;
        private IGitService _gitService;
        private IMapperVoToModel _mapperVoToModel;

        public ProjectContentOrchestrator(
            IGitlabService gitlabService, 
            IGitService gitService, 
            IMapperVoToModel mapperVoToModel)
        {
            _gitlabService = gitlabService;
            _gitService = gitService;
            _mapperVoToModel = mapperVoToModel;
        }

        public async Task ValidateActualBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo)
        {
            GitlabIssue issue = _mapperVoToModel.Mapper.Map<GitlabIssue>(issueVo);
            GitlabProject currentProject = _mapperVoToModel.Mapper.Map<GitlabProject>(currentProjectVo);
            await _gitlabService.ValidateMergeRequest(issue, currentProject);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, currentProject, StepType.Done);
        }

        public async Task CreateMergeRequestActualBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo)
        {
            GitlabIssue issue = _mapperVoToModel.Mapper.Map<GitlabIssue>(issueVo);
            GitlabProject currentProject = _mapperVoToModel.Mapper.Map<GitlabProject>(currentProjectVo);
            await _gitlabService.CreateMergeRequest(issue, currentProject);
            _gitService.CheckoutOnBranch(true);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, currentProject, StepType.ToValidate);
        }

        public async Task CreateNewBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo)
        {
            GitlabIssue issue = _mapperVoToModel.Mapper.Map<GitlabIssue>(issueVo);
            GitlabProject currentProject = _mapperVoToModel.Mapper.Map<GitlabProject>(currentProjectVo);
            await _gitlabService.SetAssigneeOnCurrentIssue(issue, currentProject);
            _gitService.CreateBranchLinkedWithIssue(issue);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, currentProject, StepType.InProgress);
        }

        public async Task<List<GitlabIssueVO>> GetGitlabIssue(ProjectVO projectVo)
        {
            GitlabProject project = _mapperVoToModel.Mapper.Map<GitlabProject>(projectVo);
            List<GitlabIssue> gitlabIssues = await _gitlabService.GetAllIssuesOfCurrentSprint(project);
            return gitlabIssues
                .Select(gitlabIssue => _mapperVoToModel.Mapper.Map<GitlabIssueVO>(gitlabIssue))
                .ToList();
        }

        public async Task TestActualBranch(GitlabIssueVO issueVo, ProjectVO projectVo)
        {
            GitlabIssue issue = _mapperVoToModel.Mapper.Map<GitlabIssue>(issueVo);
            GitlabProject project = _mapperVoToModel.Mapper.Map<GitlabProject>(projectVo);
            _gitService.CheckoutOnBranch(false, issue);
            await _gitlabService.SetAssigneeOnMergeRequest(issue, project);
            await _gitlabService.AssignCorrectLabelRelatedToCurrentIssue(issue, project, StepType.Validating);
        }
     }

    public interface IProjectContentOrchestrator
    {
        Task CreateMergeRequestActualBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo);

        Task ValidateActualBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo);

        Task CreateNewBranch(GitlabIssueVO issueVo, ProjectVO currentProjectVo);

        Task<List<GitlabIssueVO>> GetGitlabIssue(ProjectVO project);

        Task TestActualBranch(GitlabIssueVO issueVo, ProjectVO projectVo);
    }
}