using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PGM.GUI.Utilities;
using PGM.GUI.ViewModel.Orchestrators;
using PGM.Model;

namespace PGM.GUI.ViewModel
{
    public class ProjectContentViewModel : SubViewModelBase
    {
        private ICommand _testActualBranchCommand;
        private ICommand _validateActualBranchCommand;
        private GitlabIssueVO _selectedIssue;
        private readonly IProjectContentOrchestrator _projectContentOrchestrator;
        private ICommand _createMergeRequestOnGitlabCommand;
        private ProjectVO _currentProject;

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

        public bool IsRefreshing { get; set; }

        public ProjectContentViewModel(IProjectContentOrchestrator projectContentOrchestrator)
        {
            _projectContentOrchestrator = projectContentOrchestrator;
        }

        public ICommand ValidateActualBranchCommand =>
            _validateActualBranchCommand ??
            (_validateActualBranchCommand = CommandFactory.CreateAsync(ValidateActualBranch, CanValidateActualBranch,
                nameof(ValidateActualBranchCommand), this));

        private bool CanValidateActualBranch()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.Validating;
        }

        private Task ValidateActualBranch()
        {
            return _projectContentOrchestrator.ValidateActualBranch(SelectedIssue, CurrentProject);
        }

        public ICommand CreateMergeRequestOnGitlabCommand =>
            _createMergeRequestOnGitlabCommand ??
            (_createMergeRequestOnGitlabCommand = CommandFactory.CreateAsync(CreateMergeRequest, CanCreateMergeRequest,
                nameof(CreateMergeRequestOnGitlabCommand), this));

        private bool CanCreateMergeRequest()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.InProgress;
        }

        private Task CreateMergeRequest()
        {
            return _projectContentOrchestrator.CreateMergeRequestActualBranch(SelectedIssue, CurrentProject);
        }

        public ICommand TestActualBranchCommand =>
            _testActualBranchCommand ??
            (_testActualBranchCommand =
                CommandFactory.Create(TestActualBranch, CanTestActualBranch, nameof(TestActualBranchCommand)));

        private ICommand _createBranchLinkedWithIssueCommand;

        public ICommand CreateBranchLinkedWithIssueCommand =>
            _createBranchLinkedWithIssueCommand ??
            (_createBranchLinkedWithIssueCommand = CommandFactory.CreateAsync(CreateBranch, CanCreateBranch,
                nameof(CreateBranchLinkedWithIssueCommand), this));

        private bool CanCreateBranch()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.Backlog;
        }

        private Task CreateBranch()
        {
            return _projectContentOrchestrator.CreateNewBranch(SelectedIssue, CurrentProject);
        }

        

        private bool CanTestActualBranch()
        {
            return SelectedIssue != null && SelectedIssue.StepType == StepTypeVO.ToValidate;
        }

        private void TestActualBranch()
        {
            throw new System.NotImplementedException();
        }

        public ObservableCollection<GitlabIssueVO> GitlabIssues { get; set; } = new ObservableCollection<GitlabIssueVO>();

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

        private async void LoadIssues(ProjectVO currentProjectVo, bool refresh = false)
        {
            if (currentProjectVo != null)
            {
                List<GitlabIssueVO> gitlabIssueVos = await _projectContentOrchestrator.GetGitlabIssue(currentProjectVo);
                
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
}