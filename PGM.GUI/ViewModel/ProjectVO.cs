using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class ProjectVO : ObservableObject
    {
        private string _id;
        private string _name;
        private string _groupId;
        private string _groupName;
        private string _repositoryPath;

        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    Set(nameof(Id), ref _id, value);
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    Set(nameof(Name), ref _name, value);
                }
            }
        }

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                if (_repositoryPath != value)
                {
                    Set(nameof(RepositoryPath), ref _repositoryPath, value);
                }
            }
        }

        public string GroupId
        {
            get { return _groupId; }
            set
            {
                if (_groupId != value)
                {
                    Set(nameof(GroupId), ref _groupId, value);
                }
            }
        }

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (_groupName != value)
                {
                    Set(nameof(GroupName), ref _groupName, value);
                }
            }
        }

        public List<GitlabIssueVO> Issues { get; set; }

        public ProjectContentViewModel Context
        {
            get
            {
                ProjectContentViewModel projectContentViewModel = PgmServiceLocator.Current.GetInstance<ProjectContentViewModel>();
                projectContentViewModel.CurrentProject = this;
                return projectContentViewModel;
            }
        }
    }
}