using System.Threading.Tasks;
using PGM.Model;
using PGM.Service;
using PGM.Service.Git;
using PGM.Service.Gitlab;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public class MainOrchestrator : IMainOrchestrator
    {
        private readonly IGitService _gitService;
        private readonly IGitlabService _gitlabService;
        private readonly IFileSystemRepository _fileSystemRepository;


        public MainOrchestrator(
            IGitService gitService, 
            IGitlabService gitlabService, 
            IFileSystemRepository fileSystemRepository)
        {
            _gitService = gitService;
            _gitlabService = gitlabService;
            _fileSystemRepository = fileSystemRepository;
        }

        public Task<bool> CheckIfGitlabProjectExist(string projectId)
        {
            return _gitlabService.ProjectExist(projectId);
        }

        public bool CheckIfGitDirectoryPathExist(string directoryPath)
        {
            return _fileSystemRepository.DirectoryExist(directoryPath + @"\.git");
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

        bool CheckIfGitDirectoryPathExist(string directoryPath);
    }
}