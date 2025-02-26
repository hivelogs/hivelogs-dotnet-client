using Hivelogs.Client.Abstractions;
using Hivelogs.Client.Configuration;
using Hivelogs.Client.Logging;
using Hivelogs.Client.Middlewares;
using Hivelogs.Client.Services;

namespace Hivelogs.Client.Extensions
{
    public static class HivelogsExtensions
    {
        public static IServiceCollection AddHivelogs(this IServiceCollection services, HivelogsClientOptions options)
        {
            services.AddSingleton(options);

            services.AddHttpClient("HivelogsLoggerClient", client =>
            {
                client.BaseAddress = new Uri(options.ApiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

            services.AddTransient<IHivelogsClient, HivelogsClient>();

            if (options.UseBatchLogging)
            {
                services.AddScoped<ILogAccumulator, BatchLogAccumulator>();
            }

            return services;
        }

        public static IApplicationBuilder UseHivelogs(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HivelogsMiddleware>();
        }
    }
}
