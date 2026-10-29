using Temporalio.Workflows;
using Rm.FleetMonitoring.Application.Common.Interfaces;
using Rm.FleetMonitoring.Domain.Events;
using Temporalio.Common;
namespace Rm.FleetMonitoring.Application.Workflows;

[Workflow]
public class MotorMaintenanceWorkflow
{
    [WorkflowRun]
    public async Task<string> RunAsync(MotorTelemetryReceivedEvent incident)
    {
        var options = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromMinutes(2),
            RetryPolicy = new RetryPolicy
            {
                InitialInterval = TimeSpan.FromSeconds(2),
                MaximumAttempts = 3
            }
        };

        await Workflow.ExecuteActivityAsync(
            (IMaintenanceActivities act) => act.NotifyPlantOperatorAsync(
                incident.MotorId,
                $"Vibração crítica detectada: {incident.RmsVibration:F2} mm/s"
            ),
            options
        );

        var workOrderCode = await Workflow.ExecuteActivityAsync(
            (IMaintenanceActivities act) => act.DispatchWorkOrderAsync(incident),
            options
        );

        await Workflow.DelayAsync(TimeSpan.FromSeconds(10));

        await Workflow.ExecuteActivityAsync(
            (IMaintenanceActivities act) => act.VerifyMotorSensorsAsync(incident.MotorId),
            options
        );

        return $"Workflow finalizado para {incident.MotorId}. OS: {workOrderCode}";
    }
}
