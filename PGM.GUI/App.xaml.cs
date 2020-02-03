using System.Windows;
using PGM.GUI.ViewModel;

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
    }
}
