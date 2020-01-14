using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

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
}
