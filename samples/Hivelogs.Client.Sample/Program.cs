using Hivelogs.Client.Configuration;
using Hivelogs.Client.Extensions;
using Hivelogs.Client.Sample.Services;

var builder = WebApplication.CreateBuilder(args);

var hivelogsOptions = builder.Configuration.GetSection("HivelogsClient").Get<HivelogsClientOptions>()
                      ?? new HivelogsClientOptions
                      {
                          ApiUrl = "https://localhost:8081",
                          ApplicationEnvironmentId = Guid.Parse("9701fbb9-5ffa-45a6-b196-54f2591124d7"),
                          UseBatchLogging = true
                      };

builder.Services.AddHivelogs(hivelogsOptions!);

builder.Services.AddOpenApi();

builder.Services.AddScoped<ITestService, TestService>();

builder.Services.AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddHivelogsLogger();

var app = builder.Build();

app.UseHivelogs();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
