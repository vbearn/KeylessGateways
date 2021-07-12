using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance.Services
{
    public class DoorEntranceService : IDoorEntranceService
    {
        private readonly ILogger<DoorEntranceService> _logger;
        private readonly IEnumerable<IDoorAccessPolicy> _doorAccessPolicies;
        private readonly IRepository<DoorEntranceHistory> _doorEntranceRepository;
        private readonly IRepository<UserDoor> _userDoorAccessPrivilegeRepository;


        public DoorEntranceService(ILogger<DoorEntranceService> logger,
            IRepository<UserDoor> userDoorAccessPrivilegeRepository,
            IRepository<DoorEntranceHistory> doorEntranceRepository,
            IEnumerable<IDoorAccessPolicy> doorAccessPolicies
        )
        {
            _logger = logger;
            _userDoorAccessPrivilegeRepository = userDoorAccessPrivilegeRepository;
            _doorEntranceRepository = doorEntranceRepository;
            _doorAccessPolicies = doorAccessPolicies;
        }

        public async Task<bool> OpenDoor(OpenDoorExtendedModel doorOpenRequest, CancellationToken cancellationToken)
        {
            if (doorOpenRequest == null || doorOpenRequest.UserId <= 0 || doorOpenRequest.DoorId <= 0)
            {
                throw new ArgumentException();
            }

            _logger.LogInformation("[Door: {doorId} User: {userId}] Open door called for ", doorOpenRequest?.DoorId,
                doorOpenRequest?.UserId);

            // checking whether the user is authorized to open the door
            var canOpenDoor = await IsAuthorizedToOpenDoorAsync(doorOpenRequest, cancellationToken);

            if (canOpenDoor)
            {
                _logger.LogInformation(
                    "[Door: {doorId} User: {userId}] Opening the door is authorized. Now opening the physical door...",
                    doorOpenRequest?.DoorId, doorOpenRequest?.UserId);

                var physicalDoorOpened = await OpenPhysicalDoorAsync(doorOpenRequest, cancellationToken);

                if (physicalDoorOpened)
                {
                    _logger.LogInformation(
                        "[Door: {doorId} User: {userId}]  Now saving the access history...",
                        doorOpenRequest?.DoorId, doorOpenRequest?.UserId);

                    await SaveUserAccessHistory(doorOpenRequest, cancellationToken);
                    return true;
                }
            }
            else
            {
                _logger.LogInformation(
                    "[Door: {doorId} User: {userId}] Not authorized to open the door.",
                    doorOpenRequest?.DoorId, doorOpenRequest?.UserId);
            }


            return false;
        }

        private Task<bool> OpenPhysicalDoorAsync(OpenDoorExtendedModel doorOpenRequest,
            CancellationToken cancellationToken)
        {
            // TODO: implement logic to open physical door
            // this can be done possibly via making an api call to the physical door opener server or IQ

            _logger.LogInformation(
                "[Door: {doorId} User: {userId}] Physical door opened successfully.",
                doorOpenRequest?.DoorId, doorOpenRequest?.UserId);
            return Task.FromResult(true);

            // optionally can return false in case the physical door could not be opened due to any hardware issues
        }

        private async Task<bool> IsAuthorizedToOpenDoorAsync(OpenDoorExtendedModel doorOpenRequest,
            CancellationToken cancellationToken)
        {
            // fetching sorted IDoorAccessPolicy rules 
            var sortedAccessPolicies = _doorAccessPolicies.ToList();
            sortedAccessPolicies.Sort((x, y) => x.Order - y.Order);

            // fetching all user doors matching the openDoor request
            // note that we will validate whether the user is opening HIS OWN DOORS in EveryoneCanOpenTheirOwnDoorPolicy
            var userDoors = await _userDoorAccessPrivilegeRepository.TableNoTracking.Where(x =>
                    x.UserId == doorOpenRequest.UserId && x.DoorId == doorOpenRequest.DoorId)
                .ToListAsync(cancellationToken);

            // matching the access request against all the IDoorAccessPolicy rules to make sure it satisfies all of them
            sortedAccessPolicies.ForEach(async policy =>
            {
                userDoors = await policy.EnsureAccessAllowedAsync(doorOpenRequest, userDoors);
            });

            if (userDoors.Any())
            {
                // user has satisfied all policy rules and still has remaining matched rules => authorization granted
                return true;
            }

            return false;
        }

        private async Task SaveUserAccessHistory(OpenDoorDto doorOpenRequest,
            CancellationToken cancellationToken)
        {
            await _doorEntranceRepository.AddAsync(new DoorEntranceHistory
            {
                DoorId = doorOpenRequest.DoorId,
                UserId = doorOpenRequest.UserId,
                EntranceTime = DateTime.Now
            }, cancellationToken);
        }
    }
}