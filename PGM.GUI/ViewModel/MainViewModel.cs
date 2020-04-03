using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
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
        private readonly IMainOrchestrator _mainOrchestrator;
        private ICommand _activatedCommand;
        private readonly IDialogCoordinatorService _dialogCoordinatorService;
        private PGMSettingVO _pgmSettingVo;
        private ICommand _showAddProjectDialogCommand;
        private ICommand _addProjectCommand;
        private ProjectVO _selectedProject;
        private CustomDialog _setupSettingsDialog;
        private CustomDialog _addProjectDialog;
        private ICommand _closeAddProjectDialogCommand;
        private ICommand _deleteCurrentProjectCommand;

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

        public ICommand DeleteCurrentProjectCommand =>
            _deleteCurrentProjectCommand ??
            (_deleteCurrentProjectCommand =
                CommandFactory.Create<ProjectVO>(DeleteCurrentProject, CanDeleteCurrentProject, nameof(DeleteCurrentProjectCommand)));

        public MainViewModel(IMapperVoToModel mapper, 
            IPgmService pgmService, 
            IMainOrchestrator mainOrchestrator, IDialogCoordinatorService dialogCoordinatorService): base(mapper)
        {
           _pgmService = pgmService;
           _mainOrchestrator = mainOrchestrator;
           _dialogCoordinatorService = dialogCoordinatorService;
           _dialogCoordinatorService.MainWindow = (MetroWindow) Application.Current.MainWindow;
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
            if (SelectedProject == null)
            {
                return false;
            }

            if (SelectedProject.Id != null)
            {
                return _mainOrchestrator.CheckIfGitlabProjectExist(SelectedProject.Id ?? "").Result
                       && _mainOrchestrator.CheckIfGitlabGroupExist(SelectedProject.GroupId ?? "").Result
                       && _mainOrchestrator.CheckIfGitDirectoryPathExist(SelectedProject.RepositoryPath);
            }

            return _mainOrchestrator.CheckIfGitlabProjectExist(SelectedProject.Id ?? "").Result
                   && _mainOrchestrator.CheckIfGitDirectoryPathExist(SelectedProject.RepositoryPath);
        }

        private async Task AddProject()
        {
            PgmSettingVo.Projects.Add(SelectedProject);
            CallMapper<PGMSetting>(PgmSettingVo, pgmSetting => _pgmService.WriteOnPgmSettings(pgmSetting));
            await CloseAddProjectDialog();
        }

        private bool CanDeleteCurrentProject(ProjectVO projectVo)
        {
            return true;
        }

        private void DeleteCurrentProject(ProjectVO projectVo)
        {
            PgmSettingVo.Projects.Remove(projectVo);
            CallMapper<PGMSetting>(PgmSettingVo, pgmSetting => _pgmService.WriteOnPgmSettings(pgmSetting));
        }

        private bool CanCloseAddProjectDialog()
        {
            return true;
        }

        private async Task CloseAddProjectDialog()
        {
            await _dialogCoordinatorService.CloseDialog(_addProjectDialog);
            _addProjectDialog = null;
            SelectedProject = new ProjectVO();
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

        public string AppVersion
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
                Version appVersion = assembly.GetName().Version;
                return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
            }
        }

        public ObservableCollection<ProjectVO> ProjectsVo { get; set; } = new ObservableCollection<ProjectVO>();

        public ICollectionView ProjectsGrouped
        {
            get
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(ProjectsVo);
                PropertyGroupDescription propertyGroupDescription = new PropertyGroupDescription(nameof(ProjectVO.GroupName));
                
                if(!view.GroupDescriptions.Contains(propertyGroupDescription))
                {
                    view.GroupDescriptions.Add(propertyGroupDescription);
                }
                
                return view;
            }
        }

        public ProjectVO SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                if (_selectedProject != value)
                {
                    Set(nameof(SelectedProject), ref _selectedProject, value);
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
            if (PgmSettingVo == null && SelectedProject == null)
            {
                PGMSetting setting = _pgmService.InitializePgm();
                
                PgmSettingVo = Mapper.Mapper.Map<PGMSettingVO>(setting);
                SelectedProject = new ProjectVO();

                if (!setting.PgmHasSetup && _setupSettingsDialog == null)
                {
                    _setupSettingsDialog = await _dialogCoordinatorService.ShowConfigSettings("SetupSettingsDialog");
                }
                else
                {
                    IEnumerable<GitlabProject> projects = await _mainOrchestrator.GetGitlabProjectsFromCurrentUser();
                    CallMapper<List<ProjectVO>>(projects, projectsVo =>
                        projectsVo.ForEach(projectVo => ProjectsVo.Add(projectVo)));
                }
            }
        }
    }
}