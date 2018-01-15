using Devablo.Prototypes.Amazon.SQS.Receiver.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devablo.Prototypes.Amazon.SQS.Receiver.Services
{
    public interface ITestService
    {
        void Run();
    }

    public class TestService : ITestService
    {
        private readonly ILogger<TestService> _logger;
        private readonly AppSettings _config;

        public TestService(ILogger<TestService> logger, IOptions<AppSettings> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public void Run()
        {
            _logger.LogWarning($"Running Test Service");
        }
    }
}
