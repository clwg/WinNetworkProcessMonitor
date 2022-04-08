using System.Runtime.Versioning;

namespace WinNetworkProcessMonitor
{
    public sealed class WindowsBackgroundService : BackgroundService
    {
        private readonly MonitoringService _monitoringService;
        private readonly ILogger<WindowsBackgroundService> _logger;

        public WindowsBackgroundService(
            MonitoringService monitoringService,
            ILogger<WindowsBackgroundService> logger) =>
            (_monitoringService, _logger) = (monitoringService, logger);

        [SupportedOSPlatform("windows")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _monitoringService.Initialize();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.ToString());
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            }
        }
    }
}