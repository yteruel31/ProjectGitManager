using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using NLog;
using Squirrel;

namespace PGM.GUI.ViewModel.Services
{
    public class SquirrelService : ISquirrelService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDialogCoordinatorService _dialogCoordinatorService;

        public SquirrelService(IDialogCoordinatorService dialogCoordinatorService)
        {
            _dialogCoordinatorService = dialogCoordinatorService;
            _dialogCoordinatorService.MainWindow = (MetroWindow)Application.Current.MainWindow;
        }

        public async Task AutoUpdate()
        {
            using (UpdateManager updateManager =
                await UpdateManager.GitHubUpdateManager("https://github.com/yteruel31/projectgitmanager"))
            {
                UpdateInfo updateInfo = await updateManager.CheckForUpdate();
                
                if (updateInfo.ReleasesToApply.Any())
                {
                    await updateManager.UpdateApp();

                    await _dialogCoordinatorService.ShowOkCancel("New Version", $"New version ({updateInfo.FutureReleaseEntry.Version}) !");
                    updateManager.KillAllExecutablesBelongingToPackage();

                    ProcessModule processModule = Process.GetCurrentProcess().MainModule;
                    
                    if (processModule != null)
                    {
                        string currentProcessPath = processModule.FileName;
                        FileInfo fileInfo = new FileInfo(currentProcessPath);
                        DirectoryInfo directory = fileInfo.Directory;
                        FileInfo fileToLaunch = directory?.Parent?.GetFiles(fileInfo.Name).FirstOrDefault();
                            currentProcessPath = fileToLaunch?.FullName ?? currentProcessPath;
                        Process.Start(currentProcessPath, Guid.NewGuid().ToString());
                    }

                    Application.Current.Shutdown();
                }
            }
        }
    }
    public interface ISquirrelService
    {
        Task AutoUpdate();
    }
}

    