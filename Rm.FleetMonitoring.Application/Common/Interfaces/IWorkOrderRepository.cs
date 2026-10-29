using Rm.FleetMonitoring.Domain.Entities;

namespace Rm.FleetMonitoring.Application.Common.Interfaces;

public interface IWorkOrderRepository
{
    Task SaveAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);
}
