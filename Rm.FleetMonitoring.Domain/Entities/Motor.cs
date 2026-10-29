namespace Rm.FleetMonitoring.Domain.Entities;

public class Motor
{
    public string Id { get; private set; }
    public string Model { get; private set; }
    public string PlantId { get; private set; }
    public bool IsActive { get; private set; }

    public Motor(string id, string model, string plantId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Identificador do motor inválido.", nameof(id));

        Id = id;
        Model = model;
        PlantId = plantId;
        IsActive = true;
    }

    public void Decommission()
    {
        IsActive = false;
    }
}
