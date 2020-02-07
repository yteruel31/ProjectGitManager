using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;
using PGM.Service.Gitlab;

namespace PGM.Service
{
    public class PGMService : IPgmService
    {
       // private readonly IGitlabService _gitlabService;
        private readonly IFileSystemRepository _fileSystemRepository;

        public PGMService(IGitlabService gitlabService, IFileSystemRepository fileSystemRepository)
        {
            //_gitlabService = gitlabService;
            _fileSystemRepository = fileSystemRepository;
        }

        public PGMSetting InitializePgm()
        {
            FileSystemResult<PGMSetting> result = _fileSystemRepository.ReadOnFileData<PGMSetting>();
            
            if (!result.HasSucceeded)
            {
                PGMSetting pgmSetting = InitPgmSetting();
                _fileSystemRepository.WriteOnFileData(pgmSetting);
                result = _fileSystemRepository.ReadOnFileData<PGMSetting>();
            }

            if (!string.IsNullOrEmpty(result.Type.GitLabApiKey))
            {
                result.Type.PgmHasSetup = true;
            }

            return result.Type;
        }

        public void WriteOnPgmSettings(PGMSetting pgmSetting)
        {
            _fileSystemRepository.WriteOnFileData(pgmSetting);
        }

        private static PGMSetting InitPgmSetting()
        {
            PGMSetting pgmSetting = new PGMSetting
            {
                Email = string.Empty,
                FullName = string.Empty,
                GitLabApiKey = string.Empty,
                Credential = new Credential
                {
                    Username = string.Empty,
                    Password = string.Empty
                }
            };

            return pgmSetting;
        }

        /*public async Task LoadIssuesFromCurrentProject(GitlabProject project)
        {
            List<GitlabIssue> gitlabIssues = await _gitlabService.GetAllIssuesOfCurrentSprint();
            project.Issues = gitlabIssues;
        }*/
    }

    public interface IPgmService
    {
        PGMSetting InitializePgm();

        void WriteOnPgmSettings(PGMSetting pgmSetting);

        //Task LoadIssuesFromCurrentProject(GitlabProject project);
    }
}