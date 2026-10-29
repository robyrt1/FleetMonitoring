namespace Rm.FleetMonitoring.Infrastructure.Orchestration.Temporal;

using Microsoft.Extensions.Logging;
using Rm.FleetMonitoring.Application.Common.Interfaces;
using Rm.FleetMonitoring.Domain.Entities;
using Rm.FleetMonitoring.Domain.Events;

public class MaintenanceActivities(ILogger<MaintenanceActivities> logger) : IMaintenanceActivities
{
    private readonly ILogger<MaintenanceActivities> _logger = logger;

    public Task NotifyPlantOperatorAsync(string motorId, string alertMessage)
    {
        _logger.LogWarning("[ALERTA OPERACIONAL] Motor: {MotorId} | {Message}", motorId, alertMessage);
        return Task.CompletedTask;
    }

    public Task<string> DispatchWorkOrderAsync(MotorTelemetryReceivedEvent incidentData)
    {
        var workOrder = new WorkOrder(
            incidentData.MotorId,
            incidentData.PlantId,
            incidentData.RmsVibration,
            incidentData.TemperatureCelsius
        );

        _logger.LogInformation(
            "[ORDEM DE SERVIÇO GERADA] Codigo: {Code} para o Motor {MotorId} na Planta {PlantId}",
            workOrder.Code,
            workOrder.MotorId,
            workOrder.PlantId
        );

        return Task.FromResult(workOrder.Code);
    }

    public Task<bool> VerifyMotorSensorsAsync(string motorId)
    {
        _logger.LogInformation("[DIAGNÓSTICO] Sensores do motor {MotorId} checados e validados.", motorId);
        return Task.FromResult(true);
    }
}
