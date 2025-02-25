namespace Hivelogs.Client.Extensions
{
    public static class HivelogsLoggingExtensions
    {
        public static ILoggingBuilder AddHivelogsLogger(this ILoggingBuilder builder)
        {
            
            builder.Services.AddSingleton<ILoggerProvider, Logging.HivelogsLoggerProvider>();
            return builder;
        }
    }
}
