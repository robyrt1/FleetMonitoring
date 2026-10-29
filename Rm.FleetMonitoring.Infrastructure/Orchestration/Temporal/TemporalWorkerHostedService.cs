using Microsoft.Extensions.Hosting;
using Temporalio.Client;
using Temporalio.Worker;
using Rm.FleetMonitoring.Application.Common.Interfaces;
using Rm.FleetMonitoring.Application.Workflows;

namespace Rm.FleetMonitoring.Infrastructure.Orchestration.Temporal;

public class TemporalWorkerHostedService: BackgroundService
{
    private readonly ITemporalClient _client;
    private readonly IMaintenanceActivities _activities;

    public TemporalWorkerHostedService(ITemporalClient client, IMaintenanceActivities activities)
    {
        _client = client;
        _activities = activities;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var worker = new TemporalWorker(
            _client,
            new TemporalWorkerOptions("rm-motor-maintenance-queue")
                .AddWorkflow<MotorMaintenanceWorkflow>()
                .AddAllActivities(_activities)
        );

        Console.WriteLine("[TEMPORAL WORKER] Escutando a fila 'rm-motor-maintenance-queue'...");
        await worker.ExecuteAsync(stoppingToken);
    }
}
