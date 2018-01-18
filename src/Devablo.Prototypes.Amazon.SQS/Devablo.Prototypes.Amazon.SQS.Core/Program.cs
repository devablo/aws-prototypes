using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Devablo.Prototypes.Amazon.SQS.Core
{
    public interface ICommand
    {
    }
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
    public interface ICommandDispatcher
    {
        void Execute<TCommand>(TCommand command) where TCommand : ICommand;
    }
    public interface IDependencyResolver
    {
        T Resolve<T>() where T : class;
    }
    public interface IMyService
    {
        void Save(object obj);
    }

    public class MyCommand : ICommand
    {
        public string Name { get; set; }
    }
    public class MyCommandHandler : ICommandHandler<MyCommand>
    {
        private readonly IMyService service;

        public MyCommandHandler(IMyService service)
        {
            this.service = service;
        }

        public void Handle(MyCommand command)
        {
            var dto = new
            {
                Name = command.Name
            };

            service.Save(dto);
        }
    }
    public class MyService : IMyService
    {
        public void Save(object obj)
        {
            // Do Something
        }
    }

    public class QueueMessage<T> where T : class
    {
        public QueueMessage(T obj)
        {
            Type = typeof(T).FullName;
        }

        public Guid Uid { get { return Guid.NewGuid(); } }
        public string Type { get; }
        public T QueueObject { get; }
    }
    public static class MessageQueue
    {
        private static Queue<KeyValuePair<string, string>> _queue = new Queue<KeyValuePair<string, string>>();

        public static void Queue(object queueObject)
        {
            var serialized = JsonConvert.SerializeObject(queueObject);
            _queue.Enqueue(new KeyValuePair<string, string>(queueObject.GetType().FullName, serialized));
        }
        public static object Dequeue()
        {
            var item = _queue.Dequeue();
            var type = Type.GetType(item.Key);
            var deserialized = JsonConvert.DeserializeObject(item.Value, type);
            return deserialized;
        }
    }
    public class CommandDispatcher : ICommandDispatcher
    {
        public CommandDispatcher()
        {
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            MessageQueue.Queue(command);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var service = new MyService();

            var command = new MyCommand()
            {
                Name = "Aaron Morey"
            };

            var commandDispatcher = new CommandDispatcher();
            commandDispatcher.Execute(command);

            // Mimic Command Pull / Push for Message
            var obj = MessageQueue.Dequeue() as MyCommand;

            var commandReceiver = new MyCommandHandler(service);
            commandReceiver.Handle(obj);

            Console.WriteLine("Received - " + obj.Name);
            Console.ReadLine();
        }
    }
}