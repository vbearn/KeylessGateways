using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.Services.Shared.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance.EventBus
{
    public class UserDoorCreatedConsumer : IConsumer<UserDoorCreatedUpdatedEvent>
    {
        private readonly ILogger<UserDoorCreatedConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<UserDoor> _userDoorRepository;

        public UserDoorCreatedConsumer(
            ILogger<UserDoorCreatedConsumer> logger,
            IMapper mapper, IRepository<UserDoor> userDoorRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
        }

        public async Task Consume(ConsumeContext<UserDoorCreatedUpdatedEvent> context)
        {
            _logger.LogInformation("[UserDoor: {Id} Door: {doorId} User: {userId}] UserDoorCreatedEvent received",
                context.Message?.Id, context.Message?.DoorId, context.Message?.UserId);

            var entity = _mapper.Map<UserDoor>(context.Message);
            await _userDoorRepository.AddAsync(entity, CancellationToken.None);

            _logger.LogInformation(
                "[UserDoor: {Id} Door: {doorId} User: {userId}] UserDoorCreatedEvent done",
                entity?.Id, entity?.DoorId, entity?.UserId);
        }
    }

    public class UserDoorUpdatedConsumer : IConsumer<UserDoorCreatedUpdatedEvent>
    {
        private readonly ILogger<UserDoorUpdatedConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<UserDoor> _userDoorRepository;

        public UserDoorUpdatedConsumer(
            ILogger<UserDoorUpdatedConsumer> logger,
            IMapper mapper, IRepository<UserDoor> userDoorRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
        }

        public async Task Consume(ConsumeContext<UserDoorCreatedUpdatedEvent> context)
        {
            _logger.LogInformation("[UserDoor: {Id} Door: {doorId} User: {userId}] UserDoorUpdatedEvent received",
                context.Message?.Id, context.Message?.DoorId, context.Message?.UserId);

            var entity = _mapper.Map<UserDoor>(context.Message);
            await _userDoorRepository.UpdateAsync(entity, CancellationToken.None);

            _logger.LogInformation(
                "[UserDoor: {Id} Door: {doorId} User: {userId}] UserDoorUpdatedEvent done",
                entity?.Id, entity?.DoorId, entity?.UserId);
        }
    }

    public class UserDoorDeletedConsumer : IConsumer<UserDoorDeletedEvent>
    {
        private readonly ILogger<UserDoorDeletedConsumer> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<UserDoor> _userDoorRepository;

        public UserDoorDeletedConsumer(
            ILogger<UserDoorDeletedConsumer> logger,
            IMapper mapper, IRepository<UserDoor> userDoorRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
        }

        public async Task Consume(ConsumeContext<UserDoorDeletedEvent> context)
        {
            _logger.LogInformation("[UserDoor: {Id}] UserDoorDeletedEvent received",
                context.Message?.Id);
            
            var entity = await _userDoorRepository.GetByIdAsync(CancellationToken.None, context.Message.Id);
            await _userDoorRepository.DeleteAsync(entity, CancellationToken.None);

            _logger.LogInformation(
                "[UserDoor: {Id} ] UserDoorDeletedEvent done",
                entity?.Id, entity?.DoorId, entity?.UserId);
        }
    }
}