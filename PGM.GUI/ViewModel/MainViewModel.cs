using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PGM.GUI.Utilities;
using PGM.Lib;
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

        public ICommand ActivatedCommand =>
            _activatedCommand ?? (_activatedCommand = CommandFactory.CreateAsync(Activated, CanActivate, nameof(ActivatedCommand), this));

        private bool CanActivate()
        {
            throw new NotImplementedException();
        }

        private async Task Activated()
        {
            await LoadIssues(true);
        }

        public bool IsRefreshing { get; set; }

        public async Task LoadIssues(bool refresh = false)
        {
            List<GitlabIssue> gitlabIssues = 
                await await Task.Factory.StartNew(() => _gitlabService.GetAllIssuesOfCurrentSprint());
            
            Application.Current.Dispatcher.Invoke(() =>
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