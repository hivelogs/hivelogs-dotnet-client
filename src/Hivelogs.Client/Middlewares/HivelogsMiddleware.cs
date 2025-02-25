using Hivelogs.Client.Abstractions;
using Hivelogs.Client.Configuration;
using Hivelogs.Client.Dtos;
using Hivelogs.Client.Logging;
using System.Text;

namespace Hivelogs.Client.Middlewares
{
    public class HivelogsMiddleware(RequestDelegate next, IHivelogsClient hivelogsClient, ILogger<HivelogsMiddleware> logger, HivelogsClientOptions options)
    {
        private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
        private readonly IHivelogsClient _hivelogsClient = hivelogsClient;
        private readonly ILogger<HivelogsMiddleware> _logger = logger;
        private readonly HivelogsClientOptions _options = options;

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();
            var requestPath = context.Request.Path;
            var httpMethod = context.Request.Method;
            string? requestBody = await ReadRequestBodyAsync(context);

            ILogAccumulator? accumulator = null;
            if (_options.UseBatchLogging)
            {
                accumulator = context.RequestServices.GetService<ILogAccumulator>();
                if (accumulator == null)
                {
                    accumulator = new BatchLogAccumulator();
                    context.Items["LogAccumulator"] = accumulator;
                }
            }

            var startLog = new CreateLogDto
            {
                Level = "Information",
                Message = "Request started",
                CorrelationId = correlationId,
                RequestPath = requestPath,
                HttpMethod = httpMethod,
                RequestBody = requestBody,
                TimeStamp = DateTime.UtcNow
            };

            if (!_options.UseBatchLogging)
            {
                await _hivelogsClient.SendLogAsync(startLog);
            }
            else
            {
                accumulator!.AddLog(startLog);
            }

            var originalBodyStream = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                await _next(context);
                await responseBodyStream.FlushAsync();
            }
            catch (Exception ex)
            {
                var errorLog = new CreateLogDto
                {
                    Level = "Error",
                    Message = "Unhandled exception in middleware",
                    Exception = ex.ToString(),
                    CorrelationId = correlationId,
                    RequestPath = requestPath,
                    HttpMethod = httpMethod,
                    RequestBody = requestBody,
                    TimeStamp = DateTime.UtcNow
                };
                if (!_options.UseBatchLogging)
                    await _hivelogsClient.SendLogAsync(errorLog);
                else
                    accumulator!.AddLog(errorLog);
                throw;
            }
            finally
            {
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                string? responseBody = await ReadResponseBodyAsync(responseBodyStream);
                if (originalBodyStream.CanSeek)
                {
                    originalBodyStream.SetLength(0);
                }
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBodyStream);
                if (originalBodyStream.CanSeek)
                {
                    originalBodyStream.Seek(0, SeekOrigin.Begin);
                }
                context.Response.Body = originalBodyStream;
                responseBodyStream.Dispose();

                var finishLog = new CreateLogDto
                {
                    Level = "Information",
                    Message = "Request finished",
                    CorrelationId = correlationId,
                    RequestPath = requestPath,
                    HttpMethod = httpMethod,
                    ResponseBody = responseBody,
                    ResponseStatus = context.Response.StatusCode.ToString(),
                    TimeStamp = DateTime.UtcNow
                };
                if (!_options.UseBatchLogging)
                {
                    await _hivelogsClient.SendLogAsync(finishLog);
                }
                else
                {
                    accumulator!.AddLog(finishLog);
                    var logs = accumulator.GetLogs();
                    await _hivelogsClient.SendLogsAsync(logs);
                    accumulator.Clear();
                }
            }
        }

        private static async Task<string?> ReadRequestBodyAsync(HttpContext context)
        {
            if (context.Request.ContentLength is null or 0)
                return null;

            if (context.Request.ContentLength > 1024 * 1024 * 5)
                return "Request body too large to log";

            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return body;
        }

        private static async Task<string?> ReadResponseBodyAsync(MemoryStream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }
    }
}
