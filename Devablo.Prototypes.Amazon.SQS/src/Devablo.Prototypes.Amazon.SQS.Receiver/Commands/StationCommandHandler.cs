using Amazon.SQS;
using Devablo.Prototypes.Amazon.SQS.Core;
using Devablo.Prototypes.Amazon.SQS.Core.Domain;
using Devablo.Prototypes.Amazon.SQS.Core.Models;
using Devablo.Prototypes.Amazon.SQS.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devablo.Prototypes.Amazon.SQS.Publisher.Commands
{
    public class StationCommandHandler : ICommandHandler<CreateStationCommand>, ICommandHandler<EditStationCommand>
    {
        private readonly ILogger<StationCommandHandler> _logger;
        private readonly AwsSettings _config;
        private readonly IAmazonSQS _sqs;
        private readonly IStationService _stationService;
        private const string _AWSSQSURL = "https://sqs.ap-southeast-2.amazonaws.com";

        public StationCommandHandler(IOptions<AwsSettings> config, ILogger<StationCommandHandler> logger, IAmazonSQS sqs, IStationService stationService)
        {
            _logger = logger;
            _config = config.Value;
            _sqs = sqs;
            _stationService = stationService;
        }

        public void Handle(CreateStationCommand command)
        {
            _logger.LogInformation($"Station Command Handler is Handling the Create Command with Station {command.Name}");
            _stationService.Create(command);
        }

        public void Handle(EditStationCommand command)
        {
            _logger.LogInformation($"Station Command Handler is Handling the Edit Command with Station {command.Name}");
        }
    }
}
