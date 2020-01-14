using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace PGM.GUI.ViewModel
{
    public class SubViewModelBase : ViewModelBase, ISubViewModelBase
    {
        public async Task ExecuteLongActionAsync(Func<Task> asyncAction)
        {
            IsLongActionRunning = true;
            MessengerInstance.Send(new IsProcessingMessage(true));
            await Task.Delay(20);
            await asyncAction().ContinueWith(task =>
            {
                MessengerInstance.Send(new IsProcessingMessage(false));
                IsLongActionRunning = false;
            });
        }

        public void ExecuteLongAction(Action action)
        {
            IsLongActionRunning = true;
            MessengerInstance.Send(new IsProcessingMessage(true));

            action();

            MessengerInstance.Send(new IsProcessingMessage(false));
            IsLongActionRunning = false;
        }

        public bool IsLongActionRunning { get; set; }
    }

    public interface ISubViewModelBase
    {
        Task ExecuteLongActionAsync(Func<Task> asyncAction);
    }

    public class IsProcessingMessage : MessageBase
    {
        public IsProcessingMessage(bool isProcessing)
        {
            IsProcessing = isProcessing;
        }

        public bool IsProcessing { get; }
    }
}
