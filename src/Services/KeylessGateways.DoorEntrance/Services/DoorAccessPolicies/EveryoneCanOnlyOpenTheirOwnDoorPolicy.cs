using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using Microsoft.AspNetCore.Http;

namespace KeylessGateways.DoorEntrance.Services
{
    public class EveryoneCanOnlyOpenTheirOwnDoorPolicy : IDoorAccessPolicy
    {
        public int Order => 1;

        // Only Admins are authorized to open doors on behalf of others. 
        // Non-admins should only open their own doors
        public Task<List<UserDoor>> ConfigureAllowedAccessCriteria(OpenDoorExtendedModel doorOpenRequest, List<UserDoor> userDoorAccesses)
        {
            
            if (doorOpenRequest.CurrentContextUserRole != "Admin")
            {
                // user is not an admin. filter only their own doors
                userDoorAccesses = userDoorAccesses.Where(x => x.UserId == doorOpenRequest.CurrentContextUserId).ToList();
            }

            return Task.FromResult(userDoorAccesses);
        }
    }
}