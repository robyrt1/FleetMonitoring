using Temporalio.Activities;
using Rm.FleetMonitoring.Domain.Events;

namespace Rm.FleetMonitoring.Application.Common.Interfaces;

public interface IMaintenanceActivities
{
    [Activity]
    Task NotifyPlantOperatorAsync(string motorId, string alertMessage);

    [Activity]
    Task<string> DispatchWorkOrderAsync(MotorTelemetryReceivedEvent incidentData);

    [Activity]
    Task<bool> VerifyMotorSensorsAsync(string motorId);
}
