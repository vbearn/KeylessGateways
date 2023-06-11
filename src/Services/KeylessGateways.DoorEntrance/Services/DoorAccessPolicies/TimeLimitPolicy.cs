using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;

namespace KeylessGateways.DoorEntrance.Services
{
    public class TimeLimitPolicy : IDoorAccessPolicy
    {
        public int Order => 2;

        // if the user door access is time limited, here we check whether we are in the authorized time
        public Task<List<UserDoor>> ConfigureAllowedAccessCriteria(OpenDoorExtendedModel doorOpenRequest,
            List<UserDoor> userDoorAccesses)
        {
            if (doorOpenRequest.CurrentContextUserRole != "Admin")
            {
                // user is not an admin. filter only the doors that satisfy the time limit
                var now = DateTime.Now;

                userDoorAccesses = userDoorAccesses
                    .Where(x => !x.IsTimeLimited || now >= x.StartTime && now <= x.EndTime)
                    .ToList();
            }


            return Task.FromResult(userDoorAccesses);
        }
    }
}