using System;
using System.Threading.Tasks;
using PGM.GUI.ViewModel;

namespace PGM.GUI.Utilities
{
    public static class CommandFactory
    {
        public static CustomCommand Create(Action execute, Func<bool> canExecute, string commandName)
        {
            return new CustomCommand(execute, canExecute, commandName);
        }

        public static CustomCommand<T> Create<T>(Action<T> execute, Func<T, bool> canExecute, string commandName)
        {
            return new CustomCommand<T>(execute, canExecute, commandName);
        }

        public static CustomCommand<T> CreateAsync<T>(Func<T, Task> executeAsync, Func<T, bool> canExecute,
            string commandName, ISubViewModelBase subViewModelBase)
        {
            return new CustomCommand<T>(executeAsync, canExecute, commandName, subViewModelBase);
        }

        public static CustomCommand CreateAsync(Func<Task> executeAsync, Func<bool> canExecute,
            string commandName, ISubViewModelBase subViewModelBase)
        {
            return new CustomCommand(executeAsync, canExecute, commandName, subViewModelBase);
        }
    }
}