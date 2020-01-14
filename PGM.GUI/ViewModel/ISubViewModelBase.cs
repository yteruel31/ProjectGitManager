using System;
using System.Threading.Tasks;

namespace PGM.GUI.ViewModel
{
    public interface ISubViewModelBase
    {
        Task ExecuteLongActionAsync(Func<Task> asyncAction);
    }
}