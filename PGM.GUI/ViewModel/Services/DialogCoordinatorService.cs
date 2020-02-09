using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PGM.GUI.ViewModel.Services
{
    public class DialogCoordinatorService : IDialogCoordinatorService
    {
        private readonly IDialogCoordinator _dialogCoordinator;
        private readonly ISubViewModelBase _subViewModelBase;
        private readonly MetroWindow _mainWindow;

        public DialogCoordinatorService(ISubViewModelBase subViewModelBase)
        {
            _subViewModelBase = subViewModelBase;
            _dialogCoordinator = new DialogCoordinator();
            _mainWindow = (MetroWindow)Application.Current.MainWindow;
        }


        public Task<MessageDialogResult> ShowOkCancel(string title, string message)
        {

            return _mainWindow.ShowMessageAsync(title, message);
        }

        public async Task<CustomDialog> ShowConfigSettings(string resourceName)
        {
            CustomDialog dialog = new CustomDialog(_mainWindow, new MetroDialogSettings
            {
                AnimateHide = true,
                AnimateShow = true
            })
            {
                Content = _mainWindow.Resources[resourceName]
            };

            await _mainWindow.ShowMetroDialogAsync(dialog);

            return dialog;
        }

        public Task CloseDialog(CustomDialog dialog)
        {
            return _mainWindow.HideMetroDialogAsync(dialog);
        }
    }

    public interface IDialogCoordinatorService
    {
        Task<CustomDialog> ShowConfigSettings(string resourceName);

        Task CloseDialog(CustomDialog dialog);

        Task<MessageDialogResult> ShowOkCancel(string title, string message);
    }
}