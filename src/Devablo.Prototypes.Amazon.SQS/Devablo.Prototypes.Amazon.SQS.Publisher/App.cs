using Amazon.SQS;
using Amazon.SQS.Model;
using Devablo.Prototypes.Amazon.SQS.Publisher.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devablo.Prototypes.Amazon.SQS.Publisher
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly AppSettings _config;
        private readonly IAmazonSQS _sqs;
        private const string _AWSSQSURL = "https://sqs.ap-southeast-2.amazonaws.com";

        public App(IOptions<AppSettings> config, ILogger<App> logger, IAmazonSQS sqs)
        {
            _logger = logger;
            _config = config.Value;
            _sqs = sqs;
        }

        public void Run()
        {
            _logger.LogInformation($"This is a console application for AWS Publisher with Account ID {_config.AWSAccountId}");

            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = _AWSSQSURL;
            var sqsClient = new AmazonSQSClient(sqsConfig);

            var createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = "MySQSQueue";

            var attrs = new Dictionary<string, string>();
            attrs.Add(QueueAttributeName.VisibilityTimeout, "10");
            createQueueRequest.Attributes = attrs;
            var createQueueResponse = sqsClient.CreateQueueAsync(createQueueRequest).Result;

            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = "MySQSQueue",
                QueueOwnerAWSAccountId = _config.AWSAccountId
            };
            var response = sqsClient.GetQueueUrlAsync(getQueueUrlRequest).Result;
            Console.WriteLine("Queue URL: " + response.QueueUrl);

            var sendMessageRequest = new SendMessageRequest
            {
                DelaySeconds = (int)TimeSpan.FromSeconds(5).TotalSeconds,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                  {
                    {
                      "MyNameAttribute",
                        new MessageAttributeValue { DataType = "String", StringValue = "Aaron Morey" }
                    }
                  },

                MessageBody = "Aaron Morey Published Message with Attributes",
                QueueUrl = response.QueueUrl
            };
            var sendMessageResponse = sqsClient.SendMessageAsync(sendMessageRequest).Result;

            Console.WriteLine("For message ID '" + sendMessageResponse.MessageId + "':");
            Console.WriteLine("  MD5 of message attributes: " + sendMessageResponse.MD5OfMessageAttributes);
            Console.WriteLine("  MD5 of message body: " + sendMessageResponse.MD5OfMessageBody);

            System.Console.ReadKey();
        }
    }
}
