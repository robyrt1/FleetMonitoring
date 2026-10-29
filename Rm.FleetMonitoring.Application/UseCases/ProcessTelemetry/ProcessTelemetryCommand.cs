using Rm.FleetMonitoring.Application.Common.Models;
namespace Rm.FleetMonitoring.Application.UseCases.ProcessTelemetry;

public record ProcessTelemetryCommand(TelemetryDto Telemetry);
