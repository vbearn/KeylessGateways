using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.DoorEntrance.Models;

namespace KeylessGateways.DoorEntrance.Services
{
    public interface IUserDoorAccessService
    {
        Task<bool> IsAuthorizedToOpenDoorAsync(OpenDoorExtendedModel doorOpenRequest,
          CancellationToken cancellationToken);
    }
}