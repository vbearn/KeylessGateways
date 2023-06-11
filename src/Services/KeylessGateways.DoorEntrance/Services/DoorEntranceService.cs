using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance.Services
{
    public class DoorEntranceService : IDoorEntranceService
    {
        private readonly ILogger<DoorEntranceService> _logger;
        private readonly IRepository<DoorEntranceHistory> _doorEntranceRepository;
        private readonly IUserDoorAccessService _doorEntranceAccessService;



        public DoorEntranceService(ILogger<DoorEntranceService> logger,
            IRepository<DoorEntranceHistory> doorEntranceRepository,
            IUserDoorAccessService doorEntranceAccessService)
        {
            _logger = logger;
            _doorEntranceRepository = doorEntranceRepository;
            _doorEntranceAccessService = doorEntranceAccessService;
        }

        public async Task<bool> OpenDoor(OpenDoorExtendedModel doorOpenRequest, CancellationToken cancellationToken)
        {
            if (doorOpenRequest == null || doorOpenRequest.UserId == Guid.Empty || doorOpenRequest.DoorId == Guid.Empty)
            {
                throw new ArgumentException();
            }

            _logger.LogInformation("[Door: {doorId} User: {userId}] Open door called for ", doorOpenRequest?.DoorId,
                doorOpenRequest?.UserId);

            // checking whether the user is authorized to open the door
            var canOpenDoor = await _doorEntranceAccessService.IsAuthorizedToOpenDoorAsync(doorOpenRequest, cancellationToken);

            if (canOpenDoor)
            {
                _logger.LogInformation(
                    "[Door: {doorId} User: {userId}] Opening the door is authorized. Now opening the physical door...",
                    doorOpenRequest?.DoorId, doorOpenRequest?.UserId);

                // ideally, OpeningPhysicalDoor and SaveUserAccessHistory should be wrapped inside a 
                // distributed transaction 2PC mechanism, like Outbox
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

        public IQueryable<DoorEntranceHistory> GetDoorEntranceHistory(
            string userRole, Guid userId, DoorEntranceHistoryFilterDto filterDto)
        {
            var query = _doorEntranceRepository.TableNoTracking
                .Take(100); // restricted to first 100 rows. TODO: implement pagination to access more

            if (userRole != "Admin")
            {
                // if you are not an admin, you're restricted to view only your own entrance histories
                query = query.Where(x => x.UserId == userId);
            }

            if (filterDto.StartEntranceTime.HasValue)
            {
                query = query.Where(x => x.EntranceTime >= filterDto.StartEntranceTime.Value);
            }

            if (filterDto.EndEntranceTime.HasValue)
            {
                query = query.Where(x => x.EntranceTime <= filterDto.EndEntranceTime.Value);
            }

            return query;
        }
    }
}