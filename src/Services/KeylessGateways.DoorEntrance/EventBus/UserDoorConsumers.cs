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
    public class UserDoorCreatedConsumer : IConsumer<UserDoorCreatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UserDoor> _userDoorRepository;
        private readonly ILogger<UserDoorCreatedConsumer> _logger;

        public UserDoorCreatedConsumer(
            ILogger<UserDoorCreatedConsumer> logger,
            IMapper mapper, IRepository<UserDoor> userDoorRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
        }

        public async Task Consume(ConsumeContext<UserDoorCreatedEvent> context)
        {
          
            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorCreatedEvent received", context.Message?.DoorId, context.Message?.UserId);
            
            var entity = _mapper.Map<UserDoor>(context.Message);
            await _userDoorRepository.AddAsync(entity, CancellationToken.None);

            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorCreatedEvent -> UserDoor entity created: entityId {entityId}", entity.DoorId, entity.UserId, entity.Id);
        }
    }

    public class UserDoorUpdatedConsumer : IConsumer<UserDoorUpdatedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UserDoor> _userDoorRepository;

        public UserDoorUpdatedConsumer(IMapper mapper, IRepository<UserDoor> userDoorRepository
        )
        {
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
        }

        public async Task Consume(ConsumeContext<UserDoorUpdatedEvent> context)
        {
            var entity = _mapper.Map<UserDoor>(context.Message);
            await _userDoorRepository.UpdateAsync(entity, CancellationToken.None);
        }
    }

    public class UserDoorDeletedConsumer : IConsumer<UserDoorDeletedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<UserDoor> _userDoorRepository;

        public UserDoorDeletedConsumer(IMapper mapper, IRepository<UserDoor> userDoorRepository
        )
        {
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
        }

        public async Task Consume(ConsumeContext<UserDoorDeletedEvent> context)
        {
            var entity = await _userDoorRepository.GetByIdAsync(CancellationToken.None, context.Message.Id);
            await _userDoorRepository.DeleteAsync(entity, CancellationToken.None);
        }
    }
}