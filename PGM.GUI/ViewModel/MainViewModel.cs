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
using PGM.Service;
using PGM.Service.Git;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace PGM.GUI.ViewModel
{
    public class MainViewModel : SubViewModelBase
    {
        private readonly IGitlabService _gitlabService;
        private readonly IPgmService _pgmService;
        private readonly IMapperVoToModel _mapperVoToModel;
        private ICommand _activatedCommand;
        private GitlabIssue _selectedIssue;
        private PGMSettingVO _pgmSettingVo;
        private IGitService _gitService;

        public MainViewModel(
            IMapperVoToModel mapperVoToModel, 
            IGitlabService gitlabService, 
            IGitService gitService,
            IPgmService pgmService)
        {
            _mapperVoToModel = mapperVoToModel;
            GroupedIssues.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GitlabIssue.StepType)));
            _gitlabService = gitlabService;
            _gitService = gitService;
            _pgmService = pgmService;
        }

        public ICollectionView GroupedIssues
        {
            get { return CollectionViewSource.GetDefaultView(GitlabIssues);}
        }

        public bool IsRefreshing { get; set; }

        public ObservableCollection<GitlabIssueVO> GitlabIssues { get; set; } = new ObservableCollection<GitlabIssueVO>();

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

        private ICommand _createMergeRequestOnGitlabCommand;

        public ICommand CreateMergeRequestOnGitlabCommand =>
            _createMergeRequestOnGitlabCommand ??
            (_createMergeRequestOnGitlabCommand = CommandFactory.CreateAsync(CreateMergeRequest, CanCreateMergeRequest,
                nameof(CreateMergeRequestOnGitlabCommand), this));

        private bool CanCreateMergeRequest()
        {
            return true;
        }

        private async Task CreateMergeRequest()
        {
            await _gitlabService.CreateMergeRequest(SelectedIssue);
            _gitService.CheckoutOnBranch(true);
        }

        private ICommand _testActualBranchCommand;

        public ICommand TestActualBranchCommand =>
            _testActualBranchCommand ??
            (_testActualBranchCommand =
                CommandFactory.Create(TestActualBranch, CanTestActualBranch, nameof(TestActualBranchCommand)));

        private bool CanTestActualBranch()
        {
            return true;
        }

        private void TestActualBranch()
        {
            throw new System.NotImplementedException();
        }

        private ICommand _validateActualBranchCommand;

        public ICommand ValidateActualBranchCommand =>
            _validateActualBranchCommand ??
            (_validateActualBranchCommand = CommandFactory.CreateAsync(ValidateActualBranch, CanValidateActualBranch,
                nameof(ValidateActualBranchCommand), this));

        private bool CanValidateActualBranch()
        {
            return true;
        }

        private async Task ValidateActualBranch()
        {
            await _gitlabService.ValidateMergeRequest(SelectedIssue);
        }

        public ICommand LaunchPgmCommand =>
            _activatedCommand ?? (_activatedCommand = CommandFactory.Create(LaunchPgm, CanLaunchPgm, nameof(LaunchPgmCommand)));

        private bool CanLaunchPgm()
        {
            return true;
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

        private void LaunchPgm()
        {
            _pgmService.InitializePgm();
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
            Application.Current.Dispatcher?.Invoke(() =>
            {
                bool previousIsRefreshing = IsRefreshing;
                IsRefreshing = true;
                

                IsRefreshing = previousIsRefreshing;

                if (refresh)
                {
                    GroupedIssues.Refresh();
                }
            });
        }
    }
}