using System;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using DisableWakeArmedDevices.ClassLibrary;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DisableWakeArmedDevices.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TimeSpan _delayInterval;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _delayInterval = TimeSpan.FromSeconds(configuration
                .GetValue<int>("DelayInterval"));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting application");

            CancelIfNotAdmin();

            while (!stoppingToken.IsCancellationRequested)
            {
                ExecuteMainJob();

                await Task.Delay(_delayInterval, stoppingToken);
            }

            _logger.LogInformation("Exiting application");
        }

        private void ExecuteMainJob()
        {
            try
            {
                var devices = WakeDevice.GetDevicesWithWake();

                if (!devices.Any())
                    return;

                WakeDevice.DisableDevicesWake(devices);

                _logger.LogInformation("Disabled {devicesCount} devices. Devices: {devicesDisabled}",
                    devices.Count,
                    string.Join(", ", devices));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to query/disable devices");
            }
        }

        private void CancelIfNotAdmin()
        {
            if (PolicyChecker.IsAdmin())
                return;

            _logger.LogCritical("Application must be run as an Administrator!");
            throw new SecurityException("Application does not have administrative permissions");
        }
    }
}