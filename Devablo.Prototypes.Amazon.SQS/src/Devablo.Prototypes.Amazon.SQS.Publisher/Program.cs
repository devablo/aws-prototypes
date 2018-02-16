using Amazon.SQS;
using Devablo.Prototypes.Amazon.SQS.Core;
using Devablo.Prototypes.Amazon.SQS.Core.Domain;
using Devablo.Prototypes.Amazon.SQS.Core.Models;
using Devablo.Prototypes.Amazon.SQS.Publisher.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Devablo.Prototypes.Amazon.SQS.Publisher
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

            var createCommand = new CreateStationCommand();
            createCommand.Code = "2dayfm";
            createCommand.Name = "2DayFM - Sydney Hit Radio";

            // run app
            var stationCommandPublisher = serviceProvider.GetService<StationCommandPublisher>();
            stationCommandPublisher.Execute(createCommand);

            var editCommand = new EditStationCommand();
            editCommand.Uid = Guid.NewGuid();
            editCommand.Code = "fox";
            editCommand.Name = "Fox - Melbourne Hit Radio";

            stationCommandPublisher.Execute(editCommand);

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
            //serviceCollection.Configure<AppSettings>(configuration.GetSection("AWSAccountId"));
            serviceCollection.AddDefaultAWSOptions(configuration.GetAWSOptions());
            serviceCollection.AddAWSService<IAmazonSQS>();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<AwsSettings>(appSettings);

            // add app
            serviceCollection.AddTransient<ICommandPublisher<CreateStationCommand>, StationCommandPublisher>();
            serviceCollection.AddTransient<StationCommandPublisher>();
        }
    }
}
