using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PGM.GUI.ViewModel.Services
{
    public class DialogCoordinatorService : IDialogCoordinatorService
    {
        public MetroWindow MainWindow { get; set; }

        public Task<MessageDialogResult> ShowOkCancel(string title, string message)
        {
            return MainWindow.ShowMessageAsync(title, message);
        }

        public async Task<CustomDialog> ShowConfigSettings(string resourceName)
        {
            CustomDialog dialog = new CustomDialog(MainWindow, new MetroDialogSettings
            {
                AnimateHide = true,
                AnimateShow = true
            })
            {
                Content = MainWindow.Resources[resourceName]
            };

            await MainWindow.ShowMetroDialogAsync(dialog);

            return dialog;
        }

        public Task CloseDialog(CustomDialog dialog)
        {
            return MainWindow.HideMetroDialogAsync(dialog);
        }
    }

    public interface IDialogCoordinatorService
    {
        Task<CustomDialog> ShowConfigSettings(string resourceName);

        MetroWindow MainWindow { get; set; }

        Task CloseDialog(CustomDialog dialog);

        Task<MessageDialogResult> ShowOkCancel(string title, string message);
    }
}