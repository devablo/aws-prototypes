using Amazon.SQS;
using Amazon.SQS.Model;
using Devablo.Prototypes.Amazon.SQS.Core;
using Devablo.Prototypes.Amazon.SQS.Core.Contracts;
using Devablo.Prototypes.Amazon.SQS.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Devablo.Prototypes.Amazon.SQS.Publisher.MessageQueue
{
   internal class QueueCommandReceiver<TCommand> : IQueueCommandReceiver<TCommand> where TCommand : ICommand
    {
        private readonly ILogger<QueueCommandReceiver<TCommand>> _logger;
        private readonly AwsSettings _config;
        private readonly IAmazonSQS _sqs;
        private readonly ICommandHandler<TCommand> _commandHandler;
        private const string _AWSSQSURL = "https://sqs.ap-southeast-2.amazonaws.com";

        public QueueCommandReceiver(AwsSettings config, ILogger<QueueCommandReceiver<TCommand>> logger, IAmazonSQS sqs, ICommandHandler<TCommand> commandHandler)
        {
            _logger = logger;
            _config = config;
            _sqs = sqs;
            _commandHandler = commandHandler;
        }

        public void Receive()
        {
            _logger.LogInformation($"This is a console application for AWS Receiver with Account ID {_config.AWSAccountId}");

            var queueName = typeof(TCommand).Name.Replace("Command", "");
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

                var command = JsonConvert.DeserializeObject<TCommand>(message.Body);

                _commandHandler.Handle(command);

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