using FakeItEasy;
using Hivelogs.Client.Configuration;
using Hivelogs.Client.Dtos;
using Hivelogs.Client.Services;
using Hivelogs.Client.Tests.Utils;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Hivelogs.Client.Tests
{
    public class HivelogsClientTests
    {
        private readonly IHttpClientFactory _fakeHttpClientFactory;
        private readonly HivelogsClientOptions _options;
        private readonly ILogger<HivelogsClient> _fakeLogger;
        private readonly HivelogsClient _client;

        public HivelogsClientTests()
        {
            _fakeHttpClientFactory = A.Fake<IHttpClientFactory>();
            _options = new HivelogsClientOptions
            {
                ApiUrl = "https://api.hivelogs.com",
                ApplicationEnvironmentId = Guid.NewGuid()
            };
            _fakeLogger = A.Fake<ILogger<HivelogsClient>>();
            _client = new HivelogsClient(_fakeHttpClientFactory, _options, _fakeLogger);
        }

        private HttpClient SetupHttpClient(HttpResponseMessage response)
        {
            var handler = new FakeHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);

            A.CallTo(() => _fakeHttpClientFactory.CreateClient("HivelogsLoggerClient"))
                .Returns(httpClient);

            return httpClient;
        }

        [Fact]
        public async Task SendLogAsync_Success_LogsNotCalled()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            SetupHttpClient(response);

            // Act
            await _client.SendLogAsync(new CreateLogDto());

            // Assert
            A.CallTo(_fakeLogger).Where(
                call => call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error
            ).MustNotHaveHappened();
        }

        [Fact]
        public async Task SendLogAsync_ApiError_LogsError()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            SetupHttpClient(response);

            // Act
            await _client.SendLogAsync(new CreateLogDto());

            // Assert
            A.CallTo(_fakeLogger).Where(
                call => call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == LogLevel.Error &&
                call.GetArgument<object>(2).ToString().Contains("Failed to send log to Hivelogs API") && // Índice 2 (state)
                call.GetArgument<Exception?>(3) == null // Índice 3 (exception)
            ).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task SendLogAsync_SetsApplicationEnvironmentId()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var handler = new FakeHttpMessageHandler(response);
            var httpClient = new HttpClient(handler);
            A.CallTo(() => _fakeHttpClientFactory.CreateClient("HivelogsLoggerClient"))
                .Returns(httpClient);

            var log = new CreateLogDto();

            // Act
            await _client.SendLogAsync(log);

            // Assert
            Assert.Equal(_options.ApplicationEnvironmentId, log.ApplicationEnvironmentId);
        }

        [Fact]
        public async Task SendLogsAsync_SetsApplicationEnvironmentIdForAllLogs()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            SetupHttpClient(response);

            var logs = new List<CreateLogDto>
            {
                new(),
                new()
            };

            // Act
            await _client.SendLogsAsync(logs);

            // Assert
            Assert.All(logs, log =>
                Assert.Equal(_options.ApplicationEnvironmentId, log.ApplicationEnvironmentId)
            );
        }
    }
}