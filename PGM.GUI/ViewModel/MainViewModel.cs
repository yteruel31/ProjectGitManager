using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using PGM.GUI.AutoMapper;
using PGM.GUI.Utilities;
using PGM.GUI.ViewModel.Orchestrators;
using PGM.GUI.ViewModel.Services;
using PGM.Model;
using PGM.Service;
using Squirrel;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace PGM.GUI.ViewModel
{
    public class MainViewModel : SubViewModelBase
    {
        private readonly IPgmService _pgmService;
        private readonly IMapperVoToModel _mapperVoToModel;
        private readonly IMainOrchestrator _mainOrchestrator;
        private ICommand _activatedCommand;
        private readonly IDialogCoordinatorService _dialogCoordinatorService;
        private PGMSettingVO _pgmSettingVo;
        private ICommand _initializeSetupSettingsCommand;
        private ICommand _showAddProjectDialogCommand;
        private ICommand _addProjectCommand;
        private ProjectVO _projectVo;
        private CustomDialog _setupSettingsDialog;
        private CustomDialog _addProjectDialog;
        private ICommand _closeAddProjectDialogCommand;
        private bool _groupFieldIsVisible;

        public ICommand ShowAddProjectDialogCommand =>
            _showAddProjectDialogCommand ??
            (_showAddProjectDialogCommand =
                CommandFactory.CreateAsync(ShowAddProjectDialog, CanShowAddProjectDialog, nameof(ShowAddProjectDialogCommand), this));

        public ICommand CloseAddProjectDialogCommand =>
            _closeAddProjectDialogCommand ??
            (_closeAddProjectDialogCommand =
                CommandFactory.CreateAsync(CloseAddProjectDialog, CanCloseAddProjectDialog, nameof(ShowAddProjectDialogCommand), this));

        public ICommand AddProjectCommand =>
            _addProjectCommand ??
            (_addProjectCommand =
                CommandFactory.CreateAsync(AddProject, CanAddProject, nameof(AddProjectCommand), this));

        public ICommand InitializeSetupSettingsCommand =>
            _initializeSetupSettingsCommand ??
            (_initializeSetupSettingsCommand = CommandFactory.CreateAsync(InitializeSetupSettings,
                CanInitializeSetupSettings, nameof(InitializeSetupSettingsCommand), this));

        public MainViewModel(IMapperVoToModel mapperVoToModel, 
            IPgmService pgmService, 
            IMainOrchestrator mainOrchestrator)
        {
            _mapperVoToModel = mapperVoToModel;
           _pgmService = pgmService;
           _mainOrchestrator = mainOrchestrator;
           _dialogCoordinatorService = new DialogCoordinatorService(this);
        }

        private bool CanShowAddProjectDialog()
        {
            return true;
        }

        private async Task ShowAddProjectDialog()
        {
            if (_addProjectDialog == null)
            {
                _addProjectDialog = await _dialogCoordinatorService.ShowConfigSettings("AddProjectDialog");
            }
        }

        private bool CanAddProject()
        {
            if (GroupFieldIsVisible)
            {
                return _mainOrchestrator.CheckIfGitlabProjectExist(ProjectVo.Id ?? "").Result
                       && _mainOrchestrator.CheckIfGitlabGroupExist(ProjectVo.GroupId ?? "").Result
                       && _mainOrchestrator.CheckIfGitDirectoryPathExist(ProjectVo.RepositoryPath);
            }

            return _mainOrchestrator.CheckIfGitlabProjectExist(ProjectVo.Id ?? "").Result
                   && _mainOrchestrator.CheckIfGitDirectoryPathExist(ProjectVo.RepositoryPath);
        }

        private async Task AddProject()
        {
            GitlabProject gitlabProject = await _mainOrchestrator.GetGitlabProject(ProjectVo.Id);
            ProjectVo.Name = gitlabProject.Name;
            
            if (!GroupFieldIsVisible)
            {
                ProjectVo.GroupId = string.Empty;
            }

            PgmSettingVo.Projects.Add(ProjectVo);
            PGMSetting setting = _mapperVoToModel.Mapper.Map<PGMSetting>(PgmSettingVo);
            _pgmService.WriteOnPgmSettings(setting);
            await CloseAddProjectDialog();
        }

        private bool CanCloseAddProjectDialog()
        {
            return true;
        }

        private async Task CloseAddProjectDialog()
        {
            await _dialogCoordinatorService.CloseDialog(_addProjectDialog);
            _addProjectDialog = null;
        }

        private bool CanInitializeSetupSettings()
        {
            return true;
        }

        private async Task InitializeSetupSettings()
        {
            PGMSetting pgmSetting = _mapperVoToModel.Mapper.Map<PGMSetting>(PgmSettingVo);
            _pgmService.WriteOnPgmSettings(pgmSetting);
            await _dialogCoordinatorService.CloseDialog(_setupSettingsDialog);
        }

        public PGMSettingVO PgmSettingVo
        {
            get { return _pgmSettingVo; }
            set
            {
                if (_pgmSettingVo != value)
                {
                    Set(nameof(PgmSettingVo), ref _pgmSettingVo, value);
                }
            }
        }

        public ProjectVO ProjectVo
        {
            get { return _projectVo; }
            set
            {
                if (_projectVo != value)
                {
                    Set(nameof(ProjectVo), ref _projectVo, value);
                }
            }
        }

        public bool GroupFieldIsVisible
        {
            get { return _groupFieldIsVisible; }
            set
            {
                if (_groupFieldIsVisible != value)
                {
                    Set(nameof(GroupFieldIsVisible), ref _groupFieldIsVisible, value);
                }
            }
        }
 
        public ICommand LaunchPgmCommand =>
            _activatedCommand ?? 
            (_activatedCommand = CommandFactory.CreateAsync(LaunchPgm, CanLaunchPgm, nameof(LaunchPgmCommand), this));

        private bool CanLaunchPgm()
        {
            return true;
        }

        private async Task LaunchPgm()
        {
            if (PgmSettingVo == null && ProjectVo == null)
            {
                PGMSetting setting = _pgmService.InitializePgm();
                PgmSettingVo = _mapperVoToModel.Mapper.Map<PGMSettingVO>(setting);
                ProjectVo = new ProjectVO();

                if (!setting.PgmHasSetup && _setupSettingsDialog == null)
                {
                    _setupSettingsDialog = await _dialogCoordinatorService.ShowConfigSettings("SetupSettingsDialog");
                }
            }

            await CheckSquirrelUpdate();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task CheckSquirrelUpdate()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#if !DEBUG
            using (UpdateManager mgr = new UpdateManager($"https://gitlab.com/api/v4/projects/15819035/jobs/artifacts/release/raw/Release?job=release"))
            {
                UpdateInfo updateInfo = await mgr.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Any())
                {
                    await mgr.UpdateApp();

                    await _dialogCoordinatorService.ShowOkCancel("New Version", $"New version ({updateInfo.FutureReleaseEntry.Version}) !");
                    mgr.KillAllExecutablesBelongingToPackage();

                    string currentProcessPath = Process.GetCurrentProcess().MainModule.FileName;
                    FileInfo fileInfo = new FileInfo(currentProcessPath);
                    DirectoryInfo di = fileInfo.Directory;
                    FileInfo fileToLaunch = di.Parent.GetFiles(fileInfo.Name).FirstOrDefault();
                    currentProcessPath = fileToLaunch?.FullName ?? currentProcessPath;
                    Process.Start(currentProcessPath, Guid.NewGuid().ToString());
                    Application.Current.Shutdown();
                }
            }
#endif
        }
    }
}