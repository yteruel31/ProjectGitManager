using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class GitlabIssueVO : ObservableObject
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}