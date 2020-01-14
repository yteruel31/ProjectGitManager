using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using NLog;
using PGM.GUI.ViewModel;
// ReSharper disable StaticMemberInGenericType

namespace PGM.GUI.Utilities
{
    public class CustomCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<Task> _executeAsync;
        private readonly string _nameOfCommand;
        private readonly RelayCommand _relayCommand;
        private readonly ISubViewModelBase _subViewModelBase;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CustomCommand(Action execute, string nameOfCommand)
        {
            _execute = execute;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand(ExecuteWithLog);
        }

        public CustomCommand(Action execute, Func<bool> canExecute, string nameOfCommand)
        {
            _execute = execute;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand(ExecuteWithLog, canExecute);
        }

        public CustomCommand(Func<Task> executeAsync, string nameOfCommand, ISubViewModelBase subViewModelBase)
        {
            _executeAsync = executeAsync;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand(ExecuteWithLog);
            _subViewModelBase = subViewModelBase;
        }

        public CustomCommand(Func<Task> executeAsync, Func<bool> canExecute, string nameOfCommand, ISubViewModelBase subViewModelBase)
        {
            _executeAsync = executeAsync;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand(ExecuteWithLog, canExecute);
            _subViewModelBase = subViewModelBase;
        }

        private void ExecuteWithLog()
        {
            Logger.Info($"Exécution de la commande {_nameOfCommand}");

            _execute?.Invoke();

            if (_subViewModelBase != null)
            {
                _subViewModelBase.ExecuteLongActionAsync(() =>
                {
                    Task task = _executeAsync?.Invoke();

                    if (task != null)
                    {
                        task.ContinueWith(HandleErrors, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
                        return task;
                    }

                    return Task.CompletedTask;
                });
            }
            else
            {
                Task task = _executeAsync?.Invoke();
                task?.ContinueWith(HandleErrors);
            }
        }

        private void HandleErrors(Task task)
        {
            if (task.Exception != null)
            {
                foreach (Exception exceptionInnerException in task.Exception.InnerExceptions)
                {
                    Logger.Error(exceptionInnerException);
                }

                SynchronizationContext.Current.Post(o => throw task.Exception, null);
            }
        }

        public bool CanExecute(object parameter)
        {
            return _relayCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _relayCommand.Execute(parameter);
        }
    }

    public class CustomCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, Task> _executeAsync;
        private readonly string _nameOfCommand;
        private readonly RelayCommand<T> _relayCommand;
        private readonly ISubViewModelBase _subViewModelBase;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CustomCommand(Action<T> execute, string nameOfCommand)
        {
            _execute = execute;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand<T>(ExecuteWithLog);
        }

        public CustomCommand(Action<T> execute, Func<T, bool> canExecute, string nameOfCommand)
        {
            _execute = execute;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand<T>(ExecuteWithLog, canExecute);
        }

        public CustomCommand(Func<T, Task> executeAsync, Func<T, bool> canExecute, string nameOfCommand, ISubViewModelBase subViewModelBase)
        {
            _executeAsync = executeAsync;
            _nameOfCommand = nameOfCommand;
            _relayCommand = new RelayCommand<T>(ExecuteWithLog, canExecute);
            _subViewModelBase = subViewModelBase;
        }

        private void ExecuteWithLog(T obj)
        {
            Logger.Info($"Exécution de la commande {_nameOfCommand} avec le paramètre {obj}");

            _execute?.Invoke(obj);

            if (_subViewModelBase != null)
            {
                _subViewModelBase.ExecuteLongActionAsync(() =>
                {
                    Task task = _executeAsync?.Invoke(obj);

                    if (task != null)
                    {
                        task.ContinueWith(HandleErrors, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
                        return task;
                    }

                    return Task.CompletedTask;
                });
            }
            else
            {
                Task task = _executeAsync?.Invoke(obj);
                task?.ContinueWith(HandleErrors);
            }
        }

        private void HandleErrors(Task task)
        {
            if (task.Exception != null)
            {
                foreach (Exception exceptionInnerException in task.Exception.InnerExceptions)
                {
                    Logger.Error(exceptionInnerException);
                }

                SynchronizationContext.Current.Post(o => throw task.Exception, null);
            }
        }

        public bool CanExecute(object parameter)
        {
            return _relayCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _relayCommand.Execute(parameter);
        }
    }
}
