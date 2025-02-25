namespace Hivelogs.Client.Logging
{
    public class BatchHivelogsLoggerProvider(ILogAccumulator accumulator) : ILoggerProvider
    {
        private readonly ILogAccumulator _accumulator = accumulator;

        public ILogger CreateLogger(string categoryName)
        {
            return new BatchHivelogsLogger(categoryName, _accumulator);
        }

        public void Dispose() { }
    }
}
