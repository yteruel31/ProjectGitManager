using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGM.GUI.View.Controls
{
    public class FlowlTabControl : BaseFlowlTabControl
    {
        public FlowlTabControl()
        {
            DefaultStyleKey = typeof(FlowlTabControl);
        }
    }

    public abstract class BaseFlowlTabControl : TabControl
    {
        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand",
                typeof(ICommand),
                typeof(BaseFlowlTabControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty AddNewTabCommandProperty =
            DependencyProperty.Register("AddNewTabCommand", 
                typeof(ICommand), 
                typeof(BaseFlowlTabControl));

        public ICommand CloseTabCommand
        {
            get => (ICommand) GetValue(CloseTabCommandProperty);
            set => SetValue(CloseTabCommandProperty, value);
        }

        public ICommand AddNewTabCommand
        {
            get => (ICommand) GetValue(AddNewTabCommandProperty);
            set => SetValue(AddNewTabCommandProperty, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlowlTabItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element != item)
            {
                element.SetValue(DataContextProperty, item);
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        public void CloseThisTabItem(FlowlTabItem tabItem)
        {
            if (tabItem == null)
            {
                throw new ArgumentNullException(nameof(tabItem));
            }

            if (CloseTabCommand != null)
            {
                object closeTabCommandParameter = tabItem.CloseTabCommandParameter ?? tabItem;
                if (CloseTabCommand.CanExecute(closeTabCommandParameter))
                {
                    CloseTabCommand.Execute(closeTabCommandParameter);
                }
            }
        }
    }
}