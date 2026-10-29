namespace Rm.FleetMonitoring.Domain.Events;

public record MotorTelemetryReceivedEvent
(
    string MotorId,
    string PlantId,
    double RmsVibration,
    double TemperatureCelsius,
    DateTime TimestampUtc
);
