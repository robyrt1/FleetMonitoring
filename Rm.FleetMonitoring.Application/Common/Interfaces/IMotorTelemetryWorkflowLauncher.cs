using Rm.FleetMonitoring.Domain.Events;

namespace Rm.FleetMonitoring.Application.Common.Interfaces;

public interface IMotorTelemetryWorkflowLauncher
{
    Task<string> LaunchAsync(MotorTelemetryReceivedEvent telemetry, CancellationToken cancellationToken = default);
}
