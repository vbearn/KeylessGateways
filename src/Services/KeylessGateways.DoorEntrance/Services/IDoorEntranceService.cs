using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.DoorEntrance.Models;

namespace KeylessGateways.DoorEntrance.Services
{
    public interface IDoorEntranceService
    {
        Task<bool> OpenDoor(OpenDoorExtendedModel doorOpenRequest, CancellationToken cancellationToken);
    }
}