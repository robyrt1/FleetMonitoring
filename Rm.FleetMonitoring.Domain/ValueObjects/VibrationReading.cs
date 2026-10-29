namespace Rm.FleetMonitoring.Domain.ValueObjects;

public readonly record struct VibrationReading
{
    private const double CriticalIsoThresholdRms = 4.5;

    public double ValueRms { get; }
    public bool IsCritical => ValueRms > CriticalIsoThresholdRms;

    public VibrationReading(double valueRms)
    {
        if (valueRms < 0)
            throw new ArgumentOutOfRangeException(nameof(valueRms), "A vibração não pode assumir valores negativos.");

        ValueRms = valueRms;
    }
}
