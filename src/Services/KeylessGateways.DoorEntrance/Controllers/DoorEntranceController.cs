using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.DoorEntrance.Models;
using KeylessGateways.DoorEntrance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoorEntranceController : ControllerBase
    {
        private readonly IDoorEntranceService _doorEntranceService;
        private readonly ILogger<DoorEntranceController> _logger;
        private readonly IMapper _mapper;

        public DoorEntranceController(ILogger<DoorEntranceController> logger,
            IMapper mapper,
            IDoorEntranceService doorAccessService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _doorEntranceService = doorAccessService;
        }

        [HttpPost("open")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<ActionResult> OpenDoor([FromBody] OpenDoorDto dto,
            CancellationToken cancellationToken)
        {
            var model = new OpenDoorExtendedModel
            {
                DoorId = dto.DoorId,
                UserId = dto.UserId,
                CurrentContextUserId = Guid.Parse(HttpContext.User.FindFirstValue("Id")),
                CurrentContextUserRole = HttpContext.User.FindFirstValue(ClaimTypes.Role)
            };

            if (!await _doorEntranceService.OpenDoor(model, cancellationToken))
            {
                return Unauthorized(
                    "You are not authorized to open this door or the door does not exits / is not contactable.");
            }


            return Ok("Door opened successfully");
        }

        [HttpGet("history")]
        [ProducesResponseType((int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(List<DoorEntranceHistoryDto>), (int) HttpStatusCode.OK)]
        public virtual async Task<ActionResult<List<DoorEntranceHistoryDto>>> Get(
          [FromQuery] DoorEntranceHistoryFilterDto filterDto, CancellationToken cancellationToken)
        {
            var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var userId = Guid.Parse(HttpContext.User.FindFirstValue("Id"));

            var entranceList = await _doorEntranceService
                .GetDoorEntranceHistory(userRole, userId, filterDto)
                .ProjectTo<DoorEntranceHistoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(entranceList);
        }
    }
}