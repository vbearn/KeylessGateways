using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;

namespace KeylessGateways.DoorEntrance.Services
{
    public interface IDoorEntranceService
    {
        Task<bool> OpenDoor(OpenDoorExtendedModel doorOpenRequest, CancellationToken cancellationToken);

        IQueryable<DoorEntranceHistory> GetDoorEntranceHistory(
               string userRole, Guid userId, DoorEntranceHistoryFilterDto filterDto);
    }
}