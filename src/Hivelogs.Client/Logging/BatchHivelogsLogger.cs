using Hivelogs.Client.Dtos;

namespace Hivelogs.Client.Logging
{
    public class BatchHivelogsLogger(string categoryName, ILogAccumulator accumulator) : ILogger
    {
        private readonly string _categoryName = categoryName;
        private readonly ILogAccumulator _accumulator = accumulator;

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
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
            _accumulator.AddLog(logDto);
        }
    }
}
