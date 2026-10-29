using Temporalio.Workflows;
using Temporalio.Exceptions;
namespace Rm.FleetMonitoring.Infrastructure.Orchestration.Temporal;

using Temporalio.Client;
using Rm.FleetMonitoring.Application.Common.Interfaces;
using Rm.FleetMonitoring.Application.Workflows;
using Rm.FleetMonitoring.Domain.Events;
using Temporalio.Api.Enums.V1;

public class TemporalWorkflowLauncher : IMotorTelemetryWorkflowLauncher
{
    private readonly ITemporalClient _client;

    public TemporalWorkflowLauncher(ITemporalClient client)
    {
        _client = client;
    }

    public async Task<string> LaunchAsync(MotorTelemetryReceivedEvent telemetry, CancellationToken cancellationToken = default)
    {
        var workflowId = $"motor-maintenance-{telemetry.MotorId}";

        try
        {    
            var handle = await _client.StartWorkflowAsync(
                (MotorMaintenanceWorkflow wf) => wf.RunAsync(telemetry),
                new WorkflowOptions(
                    id: workflowId,
                    taskQueue: "rm-motor-maintenance-queue"
                )
                {
                    IdConflictPolicy = WorkflowIdConflictPolicy.Fail
                }
            );

            return handle.Id;
        }
        catch (WorkflowAlreadyStartedException)
        {
            return workflowId;
        }
    }
}