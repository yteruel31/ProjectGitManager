using System.Threading.Tasks;
using PGM.Model;
using PGM.Service.Git;
using PGM.Service.Gitlab;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public class MainOrchestrator : IMainOrchestrator
    {
        private readonly IGitService _gitService;
        private readonly IGitlabService _gitlabService;


        public MainOrchestrator(IGitService gitService, IGitlabService gitlabService)
        {
            _gitService = gitService;
            _gitlabService = gitlabService;
        }

        public Task<bool> CheckIfGitlabProjectExist(string projectId)
        {
            return _gitlabService.ProjectExist(projectId);
        }

        public Task<GitlabProject> GetGitlabProject(string projectId)
        {
            return _gitlabService.GetProject(projectId);
        }
    }

    public interface IMainOrchestrator
    {
        Task<bool> CheckIfGitlabProjectExist(string projectId);

        Task<GitlabProject> GetGitlabProject(string projectId);
    }
}