using WinNetworkProcessMonitor;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Network Process Monitor Service";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<MonitoringService>();
        services.AddHostedService<WindowsBackgroundService>();
    })
    .Build();

await host.RunAsync();