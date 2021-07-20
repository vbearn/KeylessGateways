using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KeylessGateways.Common;
using KeylessGateways.Management.Data;
using KeylessGateways.Management.Models;
using KeylessGateways.Services.Shared.EventBus;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserDoorsController : ControllerBase
    {
        private readonly ILogger<UserDoorsController> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Door> _doorsRepository;
        private readonly IRepository<UserDoor> _userDoorRepository;
        private readonly IBus _bus;

        public UserDoorsController(
            ILogger<UserDoorsController> logger,
            IMapper mapper,
            IRepository<UserDoor> userDoorRepository,
            IRepository<Door> doorsRepository,
            IBus bus
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
            _doorsRepository = doorsRepository;
            _bus = bus;
        }

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(List<UserDoorDto>), (int) HttpStatusCode.OK)]
        public virtual async Task<ActionResult<List<UserDoorDto>>> GetAll(CancellationToken cancellationToken)
        {
            var list = await _userDoorRepository.TableNoTracking
                .ProjectTo<UserDoorDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetUserDoor")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(UserDoorDto), (int) HttpStatusCode.OK)]
        public virtual async Task<ActionResult<UserDoorDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _userDoorRepository.TableNoTracking
                .ProjectTo<UserDoorDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
            {
                return NotFound();
            }

            return dto;
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public virtual async Task<ActionResult<UserDoorDto>> Create(UserDoorCreateUpdateDto dto,
            CancellationToken cancellationToken)
        {

            var doorExists = await _doorsRepository.TableNoTracking.AnyAsync(p => p.Id.Equals(dto.DoorId), cancellationToken);
            if (!doorExists )
            {
                return BadRequest("Door doesn't exist");
            }

            //TODO: check user exists via gPRC calling the Idendtity microservice


            var entity = _mapper.Map<UserDoor>(dto);
            await _userDoorRepository.AddAsync(entity, cancellationToken);

            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorCreatedEvent -> notifying the DoorEntrance microservice of the newly created user door access", entity.DoorId, entity.UserId);
            var createdEventBus = _mapper.Map<UserDoorCreatedUpdatedEvent>(entity);
            await _bus.Publish<UserDoorCreatedUpdatedEvent>(createdEventBus, cancellationToken);
            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorCreatedEvent published", entity.DoorId, entity.UserId);

            var dtoForGet = _mapper.Map<UserDoorDto>(entity);
            return CreatedAtRoute("GetUserDoor", new { id = entity.Id, cancellationToken }, dtoForGet);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public virtual async Task<ActionResult> Update(Guid id, UserDoorCreateUpdateDto dto,
            CancellationToken cancellationToken)
        {
            var entity = await _userDoorRepository.GetByIdAsync(cancellationToken, id);
            if (entity == null)
            {
                return NotFound();
            }


            _mapper.Map(dto, entity);
            await _userDoorRepository.UpdateAsync(entity, cancellationToken);

            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorUpdatedEvent -> notifying the DoorEntrance microservice of the updated user door access", entity.DoorId, entity.UserId);
            await _bus.Publish<UserDoorCreatedUpdatedEvent>(_mapper.Map<UserDoorCreatedUpdatedEvent>(entity), cancellationToken);
            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorUpdatedEvent published", entity.DoorId, entity.UserId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public virtual async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _userDoorRepository.GetByIdAsync(cancellationToken, id);
            if (entity == null)
            {
                return NotFound();
            }


            await _userDoorRepository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorDeletedEvent -> notifying the DoorEntrance microservice of the deleted user door access", entity.DoorId, entity.UserId);
            await _bus.Publish<UserDoorDeletedEvent>(_mapper.Map<UserDoorDeletedEvent>(entity) , cancellationToken);
            _logger.LogInformation("[Door: {doorId} User: {userId}] UserDoorDeletedEvent published", entity.DoorId, entity.UserId);

            return NoContent();
        }
    }
}