using System;
using System.IO;

using DisableWakeArmedDevices.WorkerService;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .UseSerilog(Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            nameof(DisableWakeArmedDevices),
            ".log"),
            rollingInterval: RollingInterval.Day)
        .CreateLogger())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();