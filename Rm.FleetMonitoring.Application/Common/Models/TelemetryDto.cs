namespace Rm.FleetMonitoring.Application.Common.Models;

public record TelemetryDto(
    string MotorId,
    string PlantId,
    double RmsVibration,
    double TemperatureCelsius,
    DateTime Timestamp
);
