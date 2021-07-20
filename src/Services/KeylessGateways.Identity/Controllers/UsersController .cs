using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KeylessGateways.Common;
using KeylessGateways.Identity.Data;
using KeylessGateways.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;

        public UsersController(
            ILogger<UsersController> logger,
            IMapper mapper,
            IRepository<User> userRepository,
            UserManager<User> userManager,
            RoleManager<Role> roleManager
        )
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public virtual async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken cancellationToken)
        {
            var list = await _userRepository.TableNoTracking.ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public virtual async Task<ActionResult<UserDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _userManager.FindByIdAsync(id.ToString());
            if (entity == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<UserDto>(entity);

            return dto;
        }

        [HttpPost]
        public virtual async Task<ActionResult> Create(UserCreateUpdateDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<User>(dto);
            var resCreate = await _userManager.CreateAsync(entity, dto.Password);
            if (!resCreate.Succeeded)
            {
                return BadRequest(string.Join(Environment.NewLine, resCreate.Errors.Select(x=>x.Description)));
            }

            if (dto.Admin)
            {
                var resRole = await _userManager.AddToRoleAsync(entity, "Admin");
                if (!resRole.Succeeded)
                {
                    return Conflict();
                }
            }

            var dtoForGet = _mapper.Map<UserDto>(entity);

            return CreatedAtRoute("GetUser", new { id = entity.Id, cancellationToken }, dtoForGet);
        }


        [HttpPut("{id}")]
        public virtual async Task<ActionResult> Update(Guid id, UserCreateUpdateDto dto, CancellationToken cancellationToken)
        {
            var entity = await _userManager.FindByIdAsync(id.ToString());

            var passwordValidation = await ValidatePassword(entity, dto.Password);
            if (!passwordValidation.Succeeded)
            {
                foreach (var error in passwordValidation.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            var entityMapped = _mapper.Map(dto, entity);

            await _userManager.UpdateAsync(entityMapped);
            ;

            return NoContent();
        }

        private async Task<IdentityResult> ValidatePassword(User user, string password)
        {
            var validations =
                _userManager.PasswordValidators.Select(v => v.ValidateAsync(_userManager, user, password));
            var results = await Task.WhenAll(validations);

            if (results.All(r => r.Succeeded))
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(results.SelectMany(t => t.Errors).ToArray());
        }

        // TODO: Delete Api
        // Should delete user related records across all microservices' DBs

    }
}