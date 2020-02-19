using System.Windows;
using PGM.GUI.ViewModel;
using PGM.GUI.ViewModel.Services;

namespace PGM.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            PgmServiceLocator.Initialise();
            base.OnStartup(e);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
#if !DEBUG
            PgmServiceLocator.Current.GetInstance<ISquirrelService>().AutoUpdate();
#endif
        }
    }
}
