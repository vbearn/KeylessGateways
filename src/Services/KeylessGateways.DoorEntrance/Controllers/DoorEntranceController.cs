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
        private readonly IDoorEntranceService _doorAccessService;
        private readonly IRepository<DoorEntranceHistory> _doorEntranceHistoryRepository;
        private readonly ILogger<DoorEntranceController> _logger;
        private readonly IMapper _mapper;

        public DoorEntranceController(ILogger<DoorEntranceController> logger,
            IMapper mapper,
            IRepository<DoorEntranceHistory> doorEntranceHistoryRepository,
            IDoorEntranceService doorAccessService
        )
        {
            _logger = logger;
            _mapper = mapper;
            _doorEntranceHistoryRepository = doorEntranceHistoryRepository;
            _doorAccessService = doorAccessService;
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
                CurrentContextUserId = long.Parse(HttpContext.User.FindFirstValue("Id")),
                CurrentContextUserRole = HttpContext.User.FindFirstValue(ClaimTypes.Role)
            };

            if (!await _doorAccessService.OpenDoor(model, cancellationToken))
            {
                // 401 and 403 are reserved for auth purposes. the next best thing is 404 for us
                return NotFound(
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

            var repo = _doorEntranceHistoryRepository.TableNoTracking
                .Take(100); // restricted to first 100 rows. TODO: implement pagination to access more

            if (userRole != "Admin")
            {
                // if you are not an admin, you're restricted to view only your own entrance histories
                var userId = long.Parse(HttpContext.User.FindFirstValue("Id"));
                repo = repo.Where(x => x.UserId == userId);
            }

            if (filterDto.StartEntranceTime.HasValue)
            {
                repo = repo.Where(x => x.EntranceTime >= filterDto.StartEntranceTime.Value);
            }

            if (filterDto.EndEntranceTime.HasValue)
            {
                repo = repo.Where(x => x.EntranceTime <= filterDto.EndEntranceTime.Value);
            }

            var list = await repo.ProjectTo<DoorEntranceHistoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }
    }
}