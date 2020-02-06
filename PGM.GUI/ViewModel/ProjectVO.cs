using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class ProjectVO : ObservableObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string RepositoryPath { get; set; }

        public List<GitlabIssueVO> Issues { get; set; }
    }
}