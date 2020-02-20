using System.Threading.Tasks;
using PGM.Model;
using PGM.Service;
using PGM.Service.Gitlab;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public class MainOrchestrator : IMainOrchestrator
    {
        private readonly IGitlabService _gitlabService;
        private readonly IFileSystemRepository _fileSystemRepository;


        public MainOrchestrator(
            IGitlabService gitlabService, 
            IFileSystemRepository fileSystemRepository)
        {
            _gitlabService = gitlabService;
            _fileSystemRepository = fileSystemRepository;
        }

        public Task<bool> CheckIfGitlabProjectExist(string projectId)
        {
            return _gitlabService.ProjectExist(projectId);
        }

        public Task<bool> CheckIfGitlabGroupExist(string groupId)
        {
            return _gitlabService.GroupExist(groupId);
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
}