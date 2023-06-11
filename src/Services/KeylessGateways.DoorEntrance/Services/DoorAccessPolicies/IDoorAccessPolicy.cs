using System.Collections.Generic;
using System.Threading.Tasks;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;

namespace KeylessGateways.DoorEntrance.Services
{
    public interface IDoorAccessPolicy
    {
        public int Order { get; }

        public Task<List<UserDoor>> ConfigureAllowedAccessCriteria(OpenDoorExtendedModel doorOpenRequest, List<UserDoor> userDoorAccesses);
    }
}