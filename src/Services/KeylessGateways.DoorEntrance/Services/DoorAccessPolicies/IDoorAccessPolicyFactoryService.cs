using System.Collections.Generic;

namespace KeylessGateways.DoorEntrance.Services
{
    public interface IDoorAccessPolicyFactoryService
    {
        List<IDoorAccessPolicy> GetSortedDoorAccessPolicies();
    }
}