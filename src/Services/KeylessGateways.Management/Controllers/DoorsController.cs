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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DoorsController : ControllerBase
    {
        private readonly IRepository<Door> _doorsRepository;
        private readonly IRepository<UserDoor> _userDoorRepository;
        private readonly ILogger<DoorsController> _logger;
        private readonly IMapper _mapper;

        public DoorsController(
            ILogger<DoorsController> logger,
            IMapper mapper,
            IRepository<UserDoor> userDoorRepository,
            IRepository<Door> doorsRepository
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userDoorRepository = userDoorRepository;
            _doorsRepository = doorsRepository;
        }

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(List<DoorDto>), (int) HttpStatusCode.OK)]
        public virtual async Task<ActionResult<List<DoorDto>>> GetAll(CancellationToken cancellationToken)
        {
            var list = await _doorsRepository.TableNoTracking.ProjectTo<DoorDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetDoor")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(DoorDto), (int) HttpStatusCode.OK)]
        public virtual async Task<ActionResult<DoorDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _doorsRepository.TableNoTracking.ProjectTo<DoorDto>(_mapper.ConfigurationProvider)
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
        public virtual async Task<ActionResult<DoorDto>> Create(DoorCreateUpdateDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Door>(dto);

            await _doorsRepository.AddAsync(entity, cancellationToken);

            var dtoForGet = _mapper.Map<DoorDto>(entity);
            return CreatedAtRoute("GetDoor", new { id = entity.Id, cancellationToken }, dtoForGet);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public virtual async Task<ActionResult> Update(Guid id, DoorCreateUpdateDto dto, CancellationToken cancellationToken)
        {
            var entity = await _doorsRepository.GetByIdAsync(cancellationToken, id);
            if (entity == null)
            {
                return NotFound();
            }

            _mapper.Map(dto, entity);

            await _doorsRepository.UpdateAsync(entity, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public virtual async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var hasUsages = await _userDoorRepository.TableNoTracking.AnyAsync(ud => ud.DoorId == id, cancellationToken);
            if (hasUsages)
            {
                return Conflict("This Door is used in some UserDoors. Delete them first before proceeding with the Delete.");
            }

            var entity = await _doorsRepository.GetByIdAsync(cancellationToken, id);
            if (entity == null)
            {
                return NotFound();
            }

            await _doorsRepository.DeleteAsync(entity, cancellationToken);

            return NoContent();
        }
    }
}