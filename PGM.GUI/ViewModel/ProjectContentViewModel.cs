using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using PGM.GUI.AutoMapper;
using PGM.GUI.Utilities;
using PGM.GUI.ViewModel.Orchestrators;
using PGM.GUI.ViewModel.Services;
using PGM.Model;

namespace PGM.GUI.ViewModel
{
    public class ProjectContentViewModel : SubViewModelBase
    {
        private ICommand _testActualBranchCommand;
        private ICommand _validateActualBranchCommand;
        private ICommand _createBranchLinkedWithIssueCommand;
        private GitlabIssueVO _selectedIssue;
        private readonly IProjectContentOrchestrator _projectContentOrchestrator;
        private readonly IDialogCoordinatorService _dialogCoordinatorService;
        private ICommand _createMergeRequestOnGitlabCommand;
        private ProjectVO _currentProject;
        private ICommand _openLinkInBrowserCommand;
        

        public ObservableCollection<GitlabIssueVO> GitlabIssues { get; set; } = new ObservableCollection<GitlabIssueVO>();

        public bool IsRefreshing { get; set; }

        public ProjectVO CurrentProject
        {
            get
            {
                LoadIssues(_currentProject, IsRefreshing);
                return _currentProject;
            }
            set
            {
                if (_currentProject != value)
                {
                    Set(nameof(CurrentProject), ref _currentProject, value);
                }
            }
        }

        public ICommand OpenLinkInBrowserCommand =>
            _openLinkInBrowserCommand ??
            (_openLinkInBrowserCommand =
                CommandFactory.Create(OpenLinkInBrowser, CanOpenLinkInBrowser, nameof(OpenLinkInBrowserCommand)));
        
        public ICommand TestActualBranchCommand =>
            _testActualBranchCommand ??
            (_testActualBranchCommand =
                CommandFactory.CreateAsync(TestActualBranch, CanTestActualBranch, nameof(TestActualBranchCommand), this));
        
        public ICommand CreateBranchLinkedWithIssueCommand =>
            _createBranchLinkedWithIssueCommand ??
            (_createBranchLinkedWithIssueCommand = CommandFactory.CreateAsync(CreateBranch, CanCreateBranch,
                nameof(CreateBranchLinkedWithIssueCommand), this));

        public ICommand CreateMergeRequestOnGitlabCommand =>
            _createMergeRequestOnGitlabCommand ??
            (_createMergeRequestOnGitlabCommand = CommandFactory.CreateAsync(CreateMergeRequest, CanCreateMergeRequest,
                nameof(CreateMergeRequestOnGitlabCommand), this));

        public ProjectContentViewModel(IProjectContentOrchestrator projectContentOrchestrator, IDialogCoordinatorService dialogCoordinatorService, IMapperVoToModel mapper) : base(mapper)
        {
            _projectContentOrchestrator = projectContentOrchestrator;
            _dialogCoordinatorService = dialogCoordinatorService;
            _dialogCoordinatorService.MainWindow = (MetroWindow)Application.Current.MainWindow;
        }

        public ICommand ValidateActualBranchCommand =>
            _validateActualBranchCommand ??
            (_validateActualBranchCommand = CommandFactory.CreateAsync(ValidateActualBranch, CanValidateActualBranch,
                nameof(ValidateActualBranchCommand), this));

        private bool CanValidateActualBranch()
        {
            return SelectedIssue != null
                   && SelectedIssue.StepType == StepTypeVO.Validating;
        }

        private async Task ValidateActualBranch()
        {
            bool haveConflits = await CallMapperAndReturnAsync<GitlabIssue, bool>(SelectedIssue,
                issue => _projectContentOrchestrator.MergeRequestFromCurrentIssueHaveConflict(issue));

            if (SelectedIssue != null)
            {
                if (!haveConflits)
                {
                    await CallMapperAsync<GitlabIssue>(SelectedIssue,
                        issue => _projectContentOrchestrator.ValidateActualBranch(issue));
                    LoadIssues(CurrentProject);
                }
                else
                {
                    await _dialogCoordinatorService.ShowOkCancel("La MergeRequest à des conflits",
                        "Veuillez les corriger avant de continuer");
                }
            }
        }

        private bool CanCreateMergeRequest()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.InProgress;
        }

        private async Task CreateMergeRequest()
        {
            if (SelectedIssue != null)
            {
                await CallMapperAsync<GitlabIssue>(SelectedIssue,
                    issue => _projectContentOrchestrator.CreateMergeRequestActualBranch(issue));
                LoadIssues(CurrentProject);
            }
        }

        private bool CanCreateBranch()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.Backlog;
        }

        private async Task CreateBranch()
        {
            if (SelectedIssue != null)
            {
                await CallMapperAsync<GitlabIssue>(SelectedIssue,
                    issue => _projectContentOrchestrator.CreateNewBranch(issue));
                LoadIssues(CurrentProject);
            }
        }

        private bool CanTestActualBranch()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.ToValidate;
        }

        private async Task TestActualBranch()
        {
            if (SelectedIssue != null)
            {
                await CallMapperAsync<GitlabIssue>(SelectedIssue, issue => 
                    _projectContentOrchestrator.TestActualBranch(issue));

                LoadIssues(CurrentProject);
            }
        }

        public ICollectionView GroupedIssues
        {
            get
            {
                ICollectionView groupedIssues = CollectionViewSource.GetDefaultView(GitlabIssues);
                groupedIssues.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GitlabIssue.StepType)));
                return groupedIssues;
            }
        }

        public GitlabIssueVO SelectedIssue
        {
            get { return _selectedIssue; }
            set
            {
                if (_selectedIssue != value)
                {
                    Set(nameof(SelectedIssue), ref _selectedIssue, value);
                }
            }
        }

        private bool CanOpenLinkInBrowser()
        {
            if(SelectedIssue != null)
            {
                return !string.IsNullOrEmpty(SelectedIssue.WebUrl);
            }

            return false;
        }

        private void OpenLinkInBrowser()
        {
            Process.Start(SelectedIssue.WebUrl);
        }

        private async void LoadIssues(ProjectVO currentProjectVo, bool refresh = false)
        {
            if (currentProjectVo == null)
            {
                return;
            }

            CallMapper<GitlabProject>(currentProjectVo, project => _projectContentOrchestrator.SetupRepositoryOnCurrentProject(project));
                
            List<GitlabIssue> issues = await CallMapperAndReturnAsync<GitlabProject, List<GitlabIssue>>(currentProjectVo,
                project => _projectContentOrchestrator.GetGitlabIssues(project));

            List<GitlabIssueVO> gitlabIssueVos = issues.Select(gitlabIssue => 
                    Mapper.Mapper.Map<GitlabIssueVO>(gitlabIssue))
                .ToList();

            gitlabIssueVos = gitlabIssueVos.OrderBy(i => (int) i.StepType).ToList();

            Application.Current.Dispatcher?.Invoke(() =>
            {
                bool previousIsRefreshing = IsRefreshing;
                IsRefreshing = true;
                if (!refresh)
                {
                    GitlabIssues.Clear();
                }

                foreach (GitlabIssueVO gitlabIssueVo in gitlabIssueVos)
                {
                    gitlabIssueVo.CurrentProject = currentProjectVo;
                    GitlabIssues.Add(gitlabIssueVo);
                }

                IsRefreshing = previousIsRefreshing;

                if (refresh)
                {
                    GroupedIssues.Refresh();
                    GroupedIssues.GroupDescriptions.Clear();
                }
            });
        }
    }
}