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

#pragma warning disable 1998
        public async Task AutoUpdate()
#pragma warning restore 1998
        {
           using (UpdateManager mgr = new UpdateManager(
                    @"https://gitlab.com/api/v4/projects/15819035/jobs/artifacts/release/raw/Release/RELEASES?job=release",
                    "PGM"))
                {
                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Any())
                    {
                        await mgr.UpdateApp();

                        await _dialogCoordinatorService.ShowOkCancel("New Version", $"New version ({updateInfo.FutureReleaseEntry.Version}) !");
                        mgr.KillAllExecutablesBelongingToPackage();

                        ProcessModule processModule = Process.GetCurrentProcess().MainModule;
                        if (processModule != null)
                        {
                            string currentProcessPath = processModule.FileName;
                            FileInfo fileInfo = new FileInfo(currentProcessPath);
                            DirectoryInfo di = fileInfo.Directory;
                            FileInfo fileToLaunch = di.Parent.GetFiles(fileInfo.Name).FirstOrDefault();
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

    