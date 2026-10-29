namespace Rm.FleetMonitoring.Domain.Entities;

public class WorkOrder
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string MotorId { get; private set; }
    public string PlantId { get; private set; }
    public double TriggerVibrationRms { get; private set; }
    public double TemperatureCelsius { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public WorkOrder(string motorId, string plantId, double triggerVibrationRms, double temperatureCelsius)
    {
        if (string.IsNullOrWhiteSpace(motorId))
            throw new ArgumentException("O identificador do motor é obrigatório.", nameof(motorId));

        if (string.IsNullOrWhiteSpace(plantId))
            throw new ArgumentException("A identificação da fábrica/planta é obrigatória.", nameof(plantId));

        Id = Guid.NewGuid();
        Code = $"OS-rm-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        MotorId = motorId;
        PlantId = plantId;
        TriggerVibrationRms = triggerVibrationRms;
        TemperatureCelsius = temperatureCelsius;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
