using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace PGM.GUI.View.Actions
{
    public class CommandTriggerAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        ///     Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(nameof(Command),
                typeof(ICommand),
                typeof(CommandTriggerAction),
                new PropertyMetadata(null, (s, e) => OnCommandChanged(s as CommandTriggerAction, e)));

        /// <summary>
        ///     Identifies the <see cref="CommandParameter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(nameof(CommandParameter),
                typeof(object),
                typeof(CommandTriggerAction),
                new PropertyMetadata(null,
                    (s, e) =>
                    {
                        CommandTriggerAction sender = s as CommandTriggerAction;
                        if (sender?.AssociatedObject != null)
                        {
                            sender.EnableDisableElement();
                        }
                    }));

        /// <summary>
        ///     Gets or sets the command that this trigger is bound to.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     Gets or sets an object that will be passed to the <see cref="Command" /> attached to this trigger.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }

        protected override void Invoke(object parameter)
        {
            if (AssociatedObject == null || AssociatedObject != null && !AssociatedObject.IsEnabled)
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
        }

        private static void OnCommandChanged(CommandTriggerAction action, DependencyPropertyChangedEventArgs e)
        {
            if (action == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand) e.OldValue).CanExecuteChanged -= action.OnCommandCanExecuteChanged;
            }

            ICommand command = (ICommand) e.NewValue;
            if (command != null)
            {
                command.CanExecuteChanged += action.OnCommandCanExecuteChanged;
            }

            action.EnableDisableElement();
        }

        protected virtual object GetCommandParameter()
        {
            return CommandParameter ?? AssociatedObject;
        }

        private void EnableDisableElement()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            ICommand command = Command;
            AssociatedObject.SetCurrentValue(UIElement.IsEnabledProperty,
                command == null || command.CanExecute(GetCommandParameter()));
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
    }
}