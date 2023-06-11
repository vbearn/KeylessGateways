using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using Microsoft.EntityFrameworkCore;

namespace KeylessGateways.DoorEntrance.Services
{
    public class UserDoorAccessService : IUserDoorAccessService
    {
        private readonly IRepository<UserDoor> _userDoorAccessRepository;
        private readonly IDoorAccessPolicyFactoryService _doorAccessPolicyFactory;


        public UserDoorAccessService(
            IRepository<UserDoor> userDoorAccessRepository,
            IDoorAccessPolicyFactoryService doorAccessPolicyFactory)
        {
            _userDoorAccessRepository = userDoorAccessRepository;
            _doorAccessPolicyFactory = doorAccessPolicyFactory;
        }

        public async Task<bool> IsAuthorizedToOpenDoorAsync(OpenDoorExtendedModel doorOpenRequest,
            CancellationToken cancellationToken)
        {
            var sortedAccessPolicies = _doorAccessPolicyFactory.GetSortedDoorAccessPolicies();

            // fetching all user doors matching the openDoor request
            // note that we will validate whether the user is opening HIS OWN DOORS in EveryoneCanOpenTheirOwnDoorPolicy
            var userDoors = await _userDoorAccessRepository.TableNoTracking.Where(x =>
                    x.UserId == doorOpenRequest.UserId && x.DoorId == doorOpenRequest.DoorId)
                .ToListAsync(cancellationToken);

            // matching the access request against all the IDoorAccessPolicy rules to make sure it satisfies all of them
            sortedAccessPolicies.ForEach(async policy =>
            {
                userDoors = await policy.ConfigureAllowedAccessCriteria(doorOpenRequest, userDoors);
            });

            if (userDoors.Any())
            {
                // user has satisfied all policy rules and still has remaining matched rules => authorization granted
                return true;
            }

            return false;
        }

    }
}