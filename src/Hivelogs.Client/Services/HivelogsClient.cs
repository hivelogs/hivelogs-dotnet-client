using Hivelogs.Client.Abstractions;
using Hivelogs.Client.Configuration;
using Hivelogs.Client.Dtos;
using System.Text;
using System.Text.Json;

namespace Hivelogs.Client.Services
{
    public class HivelogsClient(IHttpClientFactory httpClientFactory, HivelogsClientOptions options, ILogger<HivelogsClient> logger) : IHivelogsClient
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly HivelogsClientOptions _options = options;
        private readonly ILogger<HivelogsClient> _logger = logger;

        private HttpClient GetLoggerHttpClient()
        {
            return _httpClientFactory.CreateClient("HivelogsLoggerClient");
        }

        public async Task SendLogAsync(CreateLogDto log)
        {
            try
            {
                log.ApplicationEnvironmentId = _options.ApplicationEnvironmentId;
                var json = JsonSerializer.Serialize(log);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = GetLoggerHttpClient();
                var response = await client.PostAsync($"{_options.ApiUrl}/api/logs", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send log to Hivelogs API. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending log to Hivelogs API.");
            }
        }

        public async Task SendLogsAsync(IEnumerable<CreateLogDto> logs)
        {
            try
            {
                foreach (var log in logs)
                {
                    log.ApplicationEnvironmentId = _options.ApplicationEnvironmentId;
                }
                var json = JsonSerializer.Serialize(logs);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = GetLoggerHttpClient();
                var response = await client.PostAsync($"{_options.ApiUrl}/api/logs/batch", content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send batch logs to Hivelogs API. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending batch logs to Hivelogs API.");
            }
        }
    }
}
