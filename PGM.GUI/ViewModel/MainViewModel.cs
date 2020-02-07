using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using PGM.GUI.AutoMapper;
using PGM.GUI.Utilities;
using PGM.GUI.ViewModel.Orchestrators;
using PGM.GUI.ViewModel.Services;
using PGM.Model;
using PGM.Service;

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

        public MainViewModel(IMapperVoToModel mapperVoToModel, 
            IPgmService pgmService, 
            IMainOrchestrator mainOrchestrator)
        {
            _mapperVoToModel = mapperVoToModel;
           _pgmService = pgmService;
           _mainOrchestrator = mainOrchestrator;
           _dialogCoordinatorService = new DialogCoordinatorService(this);
        }

        public ICommand ShowAddProjectDialogCommand =>
            _showAddProjectDialogCommand ??
            (_showAddProjectDialogCommand =
                CommandFactory.CreateAsync(ShowAddProjectDialog, CanShowAddProjectDialog, nameof(ShowAddProjectDialogCommand), this));

        private bool CanShowAddProjectDialog()
        {
            return true;
        }

        private async Task ShowAddProjectDialog()
        {
            await _dialogCoordinatorService.ShowConfigSettings("AddProjectDialog");
        }

        public ICommand AddProjectCommand =>
            _addProjectCommand ??
            (_addProjectCommand =
                CommandFactory.CreateAsync<CustomDialog>(AddProject, CanAddProject, nameof(AddProjectCommand), this));

        private bool CanAddProject(CustomDialog sender)
        {
            return _mainOrchestrator.CheckIfGitlabProjectExist(ProjectVo.Id ?? "").Result;
        }

        private async Task AddProject(CustomDialog sender)
        {
            GitlabProject gitlabProject = await _mainOrchestrator.GetGitlabProject(ProjectVo.Id);
            ProjectVo.Name = gitlabProject.Name;
            PgmSettingVo.Projects.Add(ProjectVo);
            PGMSetting setting = _mapperVoToModel.Mapper.Map<PGMSetting>(PgmSettingVo);
            _pgmService.WriteOnPgmSettings(setting);
            await _dialogCoordinatorService.CloseDialog(sender);
        }

        public ICommand InitializeSetupSettingsCommand =>
            _initializeSetupSettingsCommand ??
            (_initializeSetupSettingsCommand = CommandFactory.CreateAsync<CustomDialog>(InitializeSetupSettings,
                CanInitializeSetupSettings, nameof(InitializeSetupSettingsCommand), this));

        private bool CanInitializeSetupSettings(CustomDialog sender)
        {
            return true;
        }

        private async Task InitializeSetupSettings(CustomDialog sender)
        {
            PGMSetting pgmSetting = _mapperVoToModel.Mapper.Map<PGMSetting>(PgmSettingVo);
            _pgmService.WriteOnPgmSettings(pgmSetting);
            await _dialogCoordinatorService.CloseDialog(sender);
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

        public ICommand LaunchPgmCommand =>
            _activatedCommand ?? 
            (_activatedCommand = CommandFactory.CreateAsync(LaunchPgm, CanLaunchPgm, nameof(LaunchPgmCommand), this));

        private bool CanLaunchPgm()
        {
            return true;
        }

        private async Task LaunchPgm()
        {
            PGMSetting setting = _pgmService.InitializePgm();
            PgmSettingVo = _mapperVoToModel.Mapper.Map<PGMSettingVO>(setting);
            ProjectVo = new ProjectVO();

            if (!setting.PgmHasSetup)
            {
                await _dialogCoordinatorService.ShowConfigSettings("SetupSettingsDialog");
            }
        }
    }
}