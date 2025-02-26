# Hivelogs.Client (DotNet)

[![NuGet](https://img.shields.io/nuget/v/MyHivelogsClient.svg)](https://www.nuget.org/packages/Hivelogs.Client/)
[![Build](https://github.com/hivelogs/hivelogs-dotnet-client/blob/stage/.github/workflows/nuget-publish.yml/badge.svg)](https://github.com/hivelogs/hivelogs-dotnet-client/actions)

## Overview

Hivelogs.Client is a .NET library that allows you to easily send logs from your application to the Hivelogs API. It supports both immediate and batch logging, and integrates seamlessly with the .NET logging system.

## Features

- **Immediate Logging:** Sends logs as soon as they are generated.
- **Batch Logging:** Accumulates logs during a request and sends them at the end.

## Installation

You can install the package via NuGet Package Manager or by running the following command in the Package Manager Console:

```powershell
Install-Package Hivelogs.Client
```

## Usage

Configure the library in your ASP.NET Core application:

1. Add the Hivelogs client in your Startup.cs or Program.cs:
 ```csharp
using Hivelogs.Client.Configuration;
using Hivelogs.Client.Extensions;

var hivelogsOptions = new HivelogsClientOptions
{
    ApiUrl = "https://hivelogs.yourdomain.com",
    ApplicationEnvironmentId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
    UseBatchLogging = true // or false for immediate logging
};

builder.Services.AddHivelogs(hivelogsOptions);
 ```

2. Configure the middleware and logging provider:
 ```csharp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddHivelogsLogger();

app.UseHivelogs();
```

3. Use the standard .NET logging (ILogger):
```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public void DoWork()
    {
        _logger.LogInformation("Work executed.");
    }
}
```

## License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/hivelogs/hivelogs-dotnet-client/blob/main/LICENSE) file for details.

## Contributing
Contributions are welcome! Please open issues and submit pull requests for improvements.
