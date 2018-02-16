namespace Devablo.Prototypes.Amazon.SQS.Core
{
    public interface ICommandPublisher<in TCommand> where TCommand : ICommand
    {
        void Execute(TCommand command);
    }
}
