using Amazon.SQS;
using Amazon.SQS.Model;
using Devablo.Prototypes.Amazon.SQS.Receiver.Models;
using Devablo.Prototypes.Amazon.SQS.Receiver.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Devablo.Prototypes.Amazon.SQS.Receiver
{
    public class App
    {
        private readonly ITestService _testService;
        private readonly ILogger<App> _logger;
        private readonly AppSettings _config;
        private readonly IAmazonSQS _sqs;
        private const string _AWSSQSURL = "https://sqs.ap-southeast-2.amazonaws.com";

        public App(ITestService testService, IOptions<AppSettings> config, ILogger<App> logger, IAmazonSQS sqs)
        {
            _testService = testService;
            _logger = logger;
            _config = config.Value;
            _sqs = sqs;
        }

        public void Run()
        {
            _logger.LogInformation($"This is a console application for AWS Receiver with Account ID {_config.AWSAccountId}");

            var sqsConfig = new AmazonSQSConfig();
            sqsConfig.ServiceURL = _AWSSQSURL;
            var sqsClient = new AmazonSQSClient(sqsConfig);

            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = "MySQSQueue",
                QueueOwnerAWSAccountId = _config.AWSAccountId
            };
            var response = sqsClient.GetQueueUrlAsync(getQueueUrlRequest).Result;

            var receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = response.QueueUrl;
            var receiveMessageResponse = sqsClient.ReceiveMessageAsync(receiveMessageRequest).Result;

            foreach (var message in receiveMessageResponse.Messages)
            {
                Console.WriteLine("For message ID '" + message.MessageId + "':");
                Console.WriteLine("  MD5 of message attributes: " + message.MD5OfMessageAttributes);
                Console.WriteLine("  message body: " + message.Body);
            }

            _testService.Run();

            System.Console.ReadKey();
        }
    }
}
