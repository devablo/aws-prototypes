namespace Devablo.Prototypes.Amazon.SQS.Core.Contracts
{
    public interface IQueueCommandReceiver<ICommand>
    {
        void Receive();
    }
}
