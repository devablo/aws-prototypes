using Amazon.SQS;
using Devablo.Prototypes.Amazon.SQS.Core;
using Devablo.Prototypes.Amazon.SQS.Core.Contracts;
using Devablo.Prototypes.Amazon.SQS.Core.Domain;
using Devablo.Prototypes.Amazon.SQS.Core.Models;
using Devablo.Prototypes.Amazon.SQS.Core.Services;
using Devablo.Prototypes.Amazon.SQS.Publisher.Commands;
using Devablo.Prototypes.Amazon.SQS.Publisher.MessageQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Devablo.Prototypes.Amazon.SQS.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            // create service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // create service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // run app
            var queueReceiver = serviceProvider.GetService<IQueueCommandReceiver<CreateStationCommand>>();
            queueReceiver.Receive();

            var editStationQueueCommandReceiver = serviceProvider.GetService<QueueCommandReceiver<EditStationCommand>>();
            editStationQueueCommandReceiver.Receive();

            System.Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // add logging
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole()
                .AddDebug());
            serviceCollection.AddLogging();

            // build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var AWSAccountId = configuration.GetValue<string>("AWSAccountId"); // From Environment Variable
            var appSettings = new AwsSettings { AWSAccountId = AWSAccountId };

            //serviceCollection.AddOptions();
            //serviceCollection.Configure<AppSettings>(configuration.GetSection("Configuration"));
            serviceCollection.AddDefaultAWSOptions(configuration.GetAWSOptions());
            serviceCollection.AddAWSService<IAmazonSQS>();

            // TODO: Make the Services Generic and non Impl specific
            // add services
            serviceCollection.AddTransient<IStationService, StationService>();
            serviceCollection.AddTransient<IQueueCommandReceiver<CreateStationCommand>, StationQueueCommandReceiver>();
            serviceCollection.AddTransient<ICommandHandler<CreateStationCommand>, StationCommandHandler>();
            serviceCollection.AddTransient<ICommandHandler<EditStationCommand>, StationCommandHandler>();
            serviceCollection.AddTransient<QueueCommandReceiver<EditStationCommand>>();
            serviceCollection.AddSingleton<AwsSettings>(appSettings);
        }
    }
}
