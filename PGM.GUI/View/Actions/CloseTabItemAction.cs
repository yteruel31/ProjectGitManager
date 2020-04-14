using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PGM.GUI.View.Controls;

namespace PGM.GUI.View.Actions
{
    public class CloseTabItemAction : CommandTriggerAction
    {
        private TabItem associatedTabItem;

        private TabItem AssociatedTabItem =>
            associatedTabItem ?? (associatedTabItem = TryFindParent<TabItem>(AssociatedObject));

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null || AssociatedObject != null && !AssociatedObject.IsEnabled)
            {
                return;
            }

            TabControl tabControl = TryFindParent<TabControl>(AssociatedObject);
            TabItem tabItem = AssociatedTabItem;
            if (tabControl == null || tabItem == null)
            {
                return;
            }

            ICommand command = Command;
            if (command != null)
            {
                object commandParameter = GetCommandParameter();
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }

            if (tabControl is FlowlTabControl && tabItem is FlowlTabItem)
            {
                BeginInvoke(tabControl,() => ((FlowlTabControl)tabControl).CloseThisTabItem((FlowlTabItem)tabItem));
            }
        }

        private static void BeginInvoke(DispatcherObject dispatcherObject, Action invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }
            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }
            dispatcherObject.Dispatcher.BeginInvoke(priority, invokeAction);
        }

        protected override object GetCommandParameter()
        {
            return CommandParameter ?? AssociatedTabItem;
        }

        private static T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                DependencyObject parentObject = GetParentObject(child);

                switch (parentObject)
                {
                    case null:
                        return null;
                    case T parent:
                        return parent;
                    default:
                        child = parentObject;
                        break;
                }
            }
        }

        private static DependencyObject GetParentObject(DependencyObject child)
        {
            switch (child)
            {
                case null:
                    return null;
                case ContentElement contentElement:
                {
                    DependencyObject parent = ContentOperations.GetParent(contentElement);
                    if (parent != null)
                    {
                        return parent;
                    }

                    return contentElement is FrameworkContentElement fce ? fce.Parent : null;
                }
            }

            DependencyObject childParent = VisualTreeHelper.GetParent(child);
            if (childParent != null)
            {
                return childParent;
            }

            if (child is FrameworkElement frameworkElement)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null)
                {
                    return parent;
                }
            }

            return null;
        }
    }
}