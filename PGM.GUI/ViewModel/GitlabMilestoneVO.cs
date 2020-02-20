using GalaSoft.MvvmLight;

namespace PGM.GUI.ViewModel
{
    public class GitlabMilestoneVO : ObservableObject
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}