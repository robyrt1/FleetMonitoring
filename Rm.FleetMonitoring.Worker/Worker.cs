namespace Rm.FleetMonitoring.Worker;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FleetMonitoring Host Worker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("FleetMonitoring Host ativo em: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}