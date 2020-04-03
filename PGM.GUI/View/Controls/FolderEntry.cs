using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Control = System.Windows.Controls.Control;

namespace PGM.GUI.View.Controls
{
    public class FolderEntry : Control
    {
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string),
            typeof(FolderEntry),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ICommand OpenDirectoryDialogCommand => new RelayCommand(OpenDirectoryDialog);

        private void OpenDirectoryDialog()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Path;
                fbd.ShowNewFolderButton = true;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Path = fbd.SelectedPath;
                    BindingExpression be = GetBindingExpression(PathProperty);
                    be?.UpdateSource();
                }
            }
        }
    }
}