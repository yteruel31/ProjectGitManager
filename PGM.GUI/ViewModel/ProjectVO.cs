using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class ProjectVO : ObservableObject
    {
        private string _id;
        private string _name;
        private string _groupId;

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

        public string RepositoryPath { get; set; }

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