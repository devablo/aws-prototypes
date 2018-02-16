using Amazon.SQS;
using Amazon.SQS.Model;
using Devablo.Prototypes.Amazon.SQS.Core;
using Devablo.Prototypes.Amazon.SQS.Core.Contracts;
using Devablo.Prototypes.Amazon.SQS.Core.Domain;
using Devablo.Prototypes.Amazon.SQS.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Devablo.Prototypes.Amazon.SQS.Publisher.MessageQueue
{
    public class StationQueueCommandReceiver : IQueueCommandReceiver<CreateStationCommand>
    {
        private readonly ILogger<StationQueueCommandReceiver> _logger;
        private readonly AwsSettings _config;
        private readonly IAmazonSQS _sqs;
        private readonly ICommandHandler<CreateStationCommand> _stationCommandHandler;
        private const string _AWSSQSURL = "https://sqs.ap-southeast-2.amazonaws.com";

        public StationQueueCommandReceiver(AwsSettings config, ILogger<StationQueueCommandReceiver> logger, IAmazonSQS sqs, ICommandHandler<CreateStationCommand> stationCommandHandler)
        {
            _logger = logger;
            _config = config;
            _sqs = sqs;
            _stationCommandHandler = stationCommandHandler;
        }

        public void Receive()
        {
            _logger.LogInformation($"This is a console application for AWS Receiver with Account ID {_config.AWSAccountId}");

            var queueName = "CreateStation";
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = _AWSSQSURL;
            var sqsClient = new AmazonSQSClient(sqsConfig);

            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = queueName,
                QueueOwnerAWSAccountId = _config.AWSAccountId
            };
            var response = sqsClient.GetQueueUrlAsync(getQueueUrlRequest).Result;

            var receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = response.QueueUrl;
            var receiveMessageResponse = sqsClient.ReceiveMessageAsync(receiveMessageRequest).Result;

            foreach (var message in receiveMessageResponse.Messages)
            {
                _logger.LogInformation("For message ID '" + message.MessageId + "':");

                var command = JsonConvert.DeserializeObject<CreateStationCommand>(message.Body);

                _stationCommandHandler.Handle(command);

                var delRequest = new DeleteMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    ReceiptHandle = message.ReceiptHandle
                };

                var delResponse = sqsClient.DeleteMessageAsync(delRequest);
            }
        }
    }
}