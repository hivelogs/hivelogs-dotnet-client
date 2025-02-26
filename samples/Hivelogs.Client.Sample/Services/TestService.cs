
using Hivelogs.Client.Abstractions;
using Hivelogs.Client.Dtos;

namespace Hivelogs.Client.Sample.Services
{
    public class TestService(ILogger<TestService> logger) : ITestService
    {
        private readonly ILogger<TestService> _logger = logger;

        public async Task DoWorkAsync()
        {
            _logger.LogInformation("Log Information");
            _logger.LogWarning("Log Warning");
            _logger.LogCritical("Log Critical");
            _logger.LogDebug("Log Debug");
            _logger.LogTrace("Log Trace");
            await Task.CompletedTask;
        }

        public async Task DoWithExceptionAsync()
        {
            try
            {
                throw new Exception("Test Exception");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test Exception");
                await Task.CompletedTask;
            }
        }
    }
}
