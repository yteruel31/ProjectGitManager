using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;
using PGM.Service.Gitlab;

namespace PGM.Service
{
    public class PGMService : IPgmService
    {
        private readonly IGitlabService _gitlabService;
        private readonly IFileSystemRepository _fileSystemRepository;

        public PGMService(IGitlabService gitlabService, IFileSystemRepository fileSystemRepository)
        {
            _gitlabService = gitlabService;
            _fileSystemRepository = fileSystemRepository;
        }

        public void InitializePgm()
        {
            FileSystemResult<PGMSetting> result = _fileSystemRepository.ReadOnFileData<PGMSetting>();
            
            if (!result.HasSucceeded)
            {
                PGMSetting pgmSetting = InitPgmSetting();
                _fileSystemRepository.WriteOnFileData(pgmSetting);
            }
        }

        private static PGMSetting InitPgmSetting()
        {
            PGMSetting pgmSetting = new PGMSetting
            {
                Email = "NA",
                FullName = "NA",
                GitLabApiKey = "NA",
                SettingsIsSetup = false,
                Projects = new List<Project>()
            };

            return pgmSetting;
        }

        public async Task LoadIssuesFromCurrentProject(Project project)
        {
            List<GitlabIssue> gitlabIssues = await _gitlabService.GetAllIssuesOfCurrentSprint();
            project.Issues = gitlabIssues;
        }
    }

    public interface IPgmService
    {
        void InitializePgm();

        Task LoadIssuesFromCurrentProject(Project project);
    }
}