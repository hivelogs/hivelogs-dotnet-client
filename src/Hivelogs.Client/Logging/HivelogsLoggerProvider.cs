using Hivelogs.Client.Abstractions;

namespace Hivelogs.Client.Logging
{
    public class HivelogsLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<IHivelogsClient> _hivelogsClient;

        public HivelogsLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _hivelogsClient = new Lazy<IHivelogsClient>(() => _serviceProvider.GetRequiredService<IHivelogsClient>());
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new HivelogsLogger(categoryName, _hivelogsClient);
        }

        public void Dispose() { }
    }
}
