namespace Rm.FleetMonitoring.Application.UseCases.ProcessTelemetry;

using Rm.FleetMonitoring.Application.Common.Interfaces;
using Rm.FleetMonitoring.Domain.Events;
using Rm.FleetMonitoring.Domain.ValueObjects;

public class ProcessTelemetryHandler
{
    private readonly IMotorTelemetryWorkflowLauncher _workflowLauncher;

    public ProcessTelemetryHandler(IMotorTelemetryWorkflowLauncher workflowLauncher)
    {
        _workflowLauncher = workflowLauncher;
    }

    public async Task HandleAsync(ProcessTelemetryCommand command, CancellationToken cancellationToken = default)
    {
        var vibration = new VibrationReading(command.Telemetry.RmsVibration);

        if (!vibration.IsCritical)
        {
            return;
        }

        var domainEvent = new MotorTelemetryReceivedEvent(
            command.Telemetry.MotorId,
            command.Telemetry.PlantId,
            command.Telemetry.RmsVibration,
            command.Telemetry.TemperatureCelsius,
            command.Telemetry.Timestamp
        );

        await _workflowLauncher.LaunchAsync(domainEvent, cancellationToken);
    }
}
