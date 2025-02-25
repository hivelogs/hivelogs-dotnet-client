using Hivelogs.Client.Abstractions;
using Hivelogs.Client.Dtos;

namespace Hivelogs.Client.Logging
{
    public class HivelogsLogger(string categoryName, Lazy<IHivelogsClient> hivelogsClient) : ILogger
    {
        private readonly string _categoryName = categoryName;
        private readonly Lazy<IHivelogsClient> _hivelogsClient = hivelogsClient;

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_categoryName.StartsWith("Microsoft.Hosting"))
                return;

            if (_categoryName.Contains("HivelogsLoggerClient"))
                return;

            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var logDto = new CreateLogDto
            {
                Level = logLevel.ToString(),
                Message = $"{_categoryName}: {message}",
                Exception = exception?.ToString(),
                CorrelationId = Guid.NewGuid().ToString(),
                TimeStamp = DateTime.UtcNow
            };

            _ = _hivelogsClient.Value.SendLogAsync(logDto);
        }
    }
}
