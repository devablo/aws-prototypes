namespace Devablo.Prototypes.Amazon.SQS.Core
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
