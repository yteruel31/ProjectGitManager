using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PGM.GUI.AutoMapper;
using PGM.GUI.Utilities;
using PGM.Service.Gitlab;
using PGM.Model;
using PGM.Service.Git;
using PGM.Service.Utilities;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace PGM.GUI.ViewModel
{
    public class MainViewModel : SubViewModelBase
    {
        private readonly IGitlabService _gitlabService;
        private readonly IPGMSettings _pgmSettings;
        private readonly IMapperVoToModel _mapperVoToModel;
        private ICommand _activatedCommand;
        private GitlabIssue _selectedIssue;
        private PGMSettingsVO _pgmSettingsVo;
        private IGitService _gitService;

        public MainViewModel(
            IPGMSettings pgmSettings, 
            IMapperVoToModel mapperVoToModel, 
            IGitlabService gitlabService, 
            IGitService gitService)
        {
            _pgmSettings = pgmSettings;
            _mapperVoToModel = mapperVoToModel;
            GroupedIssues.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GitlabIssue.StepType)));
            _gitlabService = gitlabService;
            _gitService = gitService;
        }

        public ICollectionView GroupedIssues
        {
            get { return CollectionViewSource.GetDefaultView(GitlabIssues); }
        }

        public bool IsRefreshing { get; set; }

        public ObservableCollection<GitlabIssue> GitlabIssues { get; set; } = new ObservableCollection<GitlabIssue>();

        private ICommand _createBranchLinkedWithIssueCommand;

        public ICommand CreateBranchLinkedWithIssueCommand =>
            _createBranchLinkedWithIssueCommand ??
            (_createBranchLinkedWithIssueCommand = CommandFactory.CreateAsync(CreateBranch, CanCreateBranch,
                nameof(CreateBranchLinkedWithIssueCommand), this));

        private bool CanCreateBranch()
        {
            return true;
        }

        private async Task CreateBranch()
        {
             await _gitlabService.SetAssigneeOnCurrentIssue(SelectedIssue);
             _gitService.CreateBranchLinkedWithIssue(SelectedIssue);
        }

        public ICommand CreateMergeRequestOnGitlabCommand{ get; set; }

        public ICommand TestActualBranchCommand { get; set; }

        public ICommand ValidateActualBranchCommand { get; set; }

        public ICommand LaunchPgmCommand =>
            _activatedCommand ?? (_activatedCommand = CommandFactory.CreateAsync(LaunchPgm, CanLaunchPgm, nameof(LaunchPgmCommand), this));

        private bool CanLaunchPgm()
        {
            return true;
        }

        public PGMSettingsVO PgmSettingsVo
        {
            get { return _pgmSettingsVo; }

            set
            {
                if (_pgmSettingsVo != value)
                {
                    Set(nameof(PgmSettingsVo), ref _pgmSettingsVo, value);
                }
            }
        }

        private async Task LaunchPgm()
        {
            PgmSettingsVo = _mapperVoToModel.GetPgmSettingsVo(_pgmSettings.GetPGMSettings);

            if (string.IsNullOrEmpty(_pgmSettingsVo.RepositoryPath)
                || !Directory.Exists(_pgmSettingsVo.RepositoryPath)
                || string.IsNullOrEmpty(_pgmSettingsVo.GitApiKey)
                || string.IsNullOrEmpty(_pgmSettingsVo.ProjectId))
            {
                return;
            }

            await LoadIssues();
        }

        public GitlabIssue SelectedIssue
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

        private async Task LoadIssues(bool refresh = false)
        {
            List<GitlabIssue> gitlabIssues = 
                await await Task.Factory.StartNew(() => _gitlabService.GetAllIssuesOfCurrentSprint());
            
            Application.Current.Dispatcher?.Invoke(() =>
            {
                bool previousIsRefreshing = IsRefreshing;
                IsRefreshing = true;
                if (!refresh)
                {
                    GitlabIssues.Clear();
                }

                if (!string.IsNullOrEmpty(_pgmSettingsVo.GitApiKey))
                {
                    foreach (GitlabIssue gitlabIssue in gitlabIssues)
                    {
                        GitlabIssues.Add(gitlabIssue);
                    }
                }

                IsRefreshing = previousIsRefreshing;

                if (refresh)
                {
                    GroupedIssues.Refresh();
                }
            });
        }
    }
}