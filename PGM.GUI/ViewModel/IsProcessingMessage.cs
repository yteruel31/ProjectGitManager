using GalaSoft.MvvmLight.Messaging;

namespace PGM.GUI.ViewModel
{
    public class IsProcessingMessage : MessageBase
    {
        public IsProcessingMessage(bool isProcessing)
        {
            IsProcessing = isProcessing;
        }

        public bool IsProcessing { get; }
    }
}