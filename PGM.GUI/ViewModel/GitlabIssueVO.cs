using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class GitlabIssueVO : ObservableObject
    {
        private int _id;
        private string _title;

        public int Id
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

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    Set(nameof(Title), ref _title, value);
                }
            }
        }

        public string Description { get; set; }

        public StepTypeVO StepType { get; set; }
    }
}