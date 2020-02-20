using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using PGM.GUI.AutoMapper;

namespace PGM.GUI.ViewModel
{
    public class SubViewModelBase : ViewModelBase, ISubViewModelBase
    {
        protected IMapperVoToModel Mapper { get; }

        private bool _isLongActionRunning;

        protected SubViewModelBase(IMapperVoToModel mapper)
        {
            Mapper = mapper;
        }

        protected void CallMapper<TTarget>(object vo, Action<TTarget> action)
        {
            TTarget value = Mapper.Mapper.Map<TTarget>(vo);
            action(value);
        }

        protected async Task CallMapperAsync<TTarget>(object vo, Func<TTarget, Task> action)
        {
            TTarget value = Mapper.Mapper.Map<TTarget>(vo);
            await action(value);
        }

        protected Task<TReturn> CallMapperAndReturnAsync<TTarget, TReturn>(object source, Func<TTarget, Task<TReturn>> action)
        {
            TTarget value = Mapper.Mapper.Map<TTarget>(source);
            return action(value);
        }

        public void ExecuteLongAction(Action action)
        {
            _isLongActionRunning = true;
            MessengerInstance.Send(new IsProcessingMessage(true));

            action();

            MessengerInstance.Send(new IsProcessingMessage(false));
            _isLongActionRunning = false;
        }

        public async Task ExecuteLongActionAsync(Func<Task> asyncAction)
        {
            _isLongActionRunning = true;
            MessengerInstance.Send(new IsProcessingMessage(true));
            await Task.Delay(20);
            await asyncAction().ContinueWith(task =>
            {
                MessengerInstance.Send(new IsProcessingMessage(false));
                _isLongActionRunning = false;
            });
        }
    }
}
