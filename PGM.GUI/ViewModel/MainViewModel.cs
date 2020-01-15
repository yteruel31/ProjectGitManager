using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PGM.GUI.Utilities;
using PGM.Lib.Gitlab;
using PGM.Lib.Model;
using PGM.Lib.Model.Issues;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace PGM.GUI.ViewModel
{
    public class MainViewModel : SubViewModelBase
    {
        private readonly GitlabService _gitlabService;
        private ICommand _activatedCommand;

        public MainViewModel()
        {
            GitlabIssues = new ObservableCollection<GitlabIssue>();
            GroupedIssues = CollectionViewSource.GetDefaultView(GitlabIssues);
            GroupedIssues.GroupDescriptions.Add(new PropertyGroupDescription(nameof(GitlabIssue.StepType)));
            _gitlabService = new GitlabService(new PGMSettings(SettingsViewModel));
        }

        public SettingsViewModel SettingsViewModel { get; } = new SettingsViewModel();

        public ICollectionView GroupedIssues { get; set; }

        public ObservableCollection<GitlabIssue> GitlabIssues { get; set; }

        public ICommand CreateBranchLinkedWithIssueCommand { get; set; }

        public ICommand CreateMergeRequestOnGitlabCommand{ get; set; }

        public ICommand TestActualBranchCommand { get; set; }

        public ICommand ValidateActualBranchCommand { get; set; }

        public ICommand LaunchPgmCommand =>
            _activatedCommand ?? (_activatedCommand = CommandFactory.CreateAsync(LaunchPgm, CanLaunchPgm, nameof(LaunchPgmCommand), this));

        private bool CanLaunchPgm()
        {
            return true;
        }

        private async Task LaunchPgm()
        {
            if (string.IsNullOrEmpty(SettingsViewModel.RepositoryPath)
                || !Directory.Exists(SettingsViewModel.RepositoryPath)
                || string.IsNullOrEmpty(SettingsViewModel.GitApiKey)
                || string.IsNullOrEmpty(SettingsViewModel.ProjectId))
            {
                return;
            }

            await LoadIssues();
        }

        private GitlabIssue _selectedIssue;

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

        public bool IsRefreshing { get; set; }

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

                if (!string.IsNullOrEmpty(SettingsViewModel.GitApiKey))
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