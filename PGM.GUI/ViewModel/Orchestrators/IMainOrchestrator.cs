using System.Collections.Generic;
using System.Threading.Tasks;
using PGM.Model;

namespace PGM.GUI.ViewModel.Orchestrators
{
    public interface IMainOrchestrator
    {
        Task<bool> CheckIfGitlabProjectExist(string projectId);

        Task<GitlabProject> GetGitlabProject(string projectId);

        bool CheckIfGitDirectoryPathExist(string directoryPath);

        Task<bool> CheckIfGitlabGroupExist(string groupId);

        Task<IEnumerable<GitlabProject>> GetGitlabProjectsFromCurrentUser();
    }
}