using Amazon.SQS;
using Amazon.SQS.Model;
using Devablo.Prototypes.Amazon.SQS.Core;
using Devablo.Prototypes.Amazon.SQS.Core.Domain;
using Devablo.Prototypes.Amazon.SQS.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Devablo.Prototypes.Amazon.SQS.Publisher.Commands
{
    public class StationCommandPublisher : ICommandPublisher<CreateStationCommand>, ICommandPublisher<EditStationCommand>
    {
        private readonly ILogger<StationCommandPublisher> _logger;
        private readonly AwsSettings _config;
        private readonly IAmazonSQS _sqs;
        private const string _AWSSQSURL = "https://sqs.ap-southeast-2.amazonaws.com";

        public StationCommandPublisher(AwsSettings config, ILogger<StationCommandPublisher> logger, IAmazonSQS sqs)
        {
            _logger = logger;
            _config = config;
            _sqs = sqs;
        }

        public void Execute(CreateStationCommand command)
        {
            _logger.LogInformation($"This is a console application for AWS Publisher with Account ID {_config.AWSAccountId}");
            var queueName = "CreateStation";
            PublishQueue(queueName, command);
        }

        public void Execute(EditStationCommand command)
        {
            var queueName = "EditStation";
            PublishQueue(queueName, command);
        }

        private void PublishQueue(string queueName, object payload)
        {
            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = _AWSSQSURL;
            var sqsClient = new AmazonSQSClient(sqsConfig);

            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = queueName;

            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");

            createQueueRequest.Attributes = attrs;
            var createQueueResponse = sqsClient.CreateQueueAsync(createQueueRequest).Result;

            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = queueName,
                QueueOwnerAWSAccountId = _config.AWSAccountId
            };

            var data = JsonConvert.SerializeObject(payload);
            var response = sqsClient.GetQueueUrlAsync(getQueueUrlRequest).Result;
            var sendMessageRequest = new SendMessageRequest
            {
                DelaySeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds,
                MessageBody = data,
                QueueUrl = response.QueueUrl
            };

            // TODO: Handle Response
            var sendMessageResponse = sqsClient.SendMessageAsync(sendMessageRequest).Result;
        }
    }
}
