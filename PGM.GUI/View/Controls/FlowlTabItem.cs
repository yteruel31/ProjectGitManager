using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGM.GUI.View.Controls
{
    public class FlowlTabItem : TabItem
    {
        public FlowlTabItem()
        {
            DefaultStyleKey = typeof(FlowlTabItem);
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand",
                typeof(ICommand),
                typeof(FlowlTabItem));

        public ICommand CloseTabCommand
        {
            get => (ICommand)GetValue(CloseTabCommandProperty);
            set => SetValue(CloseTabCommandProperty, value);
        }

        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.Register("CloseTabCommandParameter", 
                typeof(object), 
                typeof(FlowlTabItem));

        public object CloseTabCommandParameter
        {
            get => GetValue(CloseTabCommandParameterProperty);
            set => SetValue(CloseTabCommandParameterProperty, value);
        }
    }
}