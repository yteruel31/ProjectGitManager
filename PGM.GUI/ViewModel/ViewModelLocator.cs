namespace PGM.GUI.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel MainPgm => PgmServiceLocator.Current.GetInstance<MainViewModel>();
    }
}