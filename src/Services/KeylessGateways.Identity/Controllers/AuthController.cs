using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KeylessGateways.Identity.Infrastructure;
using KeylessGateways.Identity.Models;
using KeylessGateways.Identity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace KeylessGateways.Identity.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(ILogger<AuthController> logger, IUserService userService, IJwtAuthManager jwtAuthManager)
        {
            _logger = logger;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userCheck = await _userService.IsValidUserCredentialsAsync(request.UserName, request.Password);
            if (!userCheck.Valid)
            {
                return Unauthorized();
            }

            var roles = await _userService.GetUserRoleAsync(request.UserName);
            var claims = new List<Claim>
            {
                new Claim("Id", userCheck.Id.ToString()),
                new Claim(ClaimTypes.Name, request.UserName)
            };

            if (roles.Any())
            {
                claims.Add(
                    new Claim(ClaimTypes.Role, roles.First())
                );
            }

            var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims.ToArray(), DateTime.Now);
            _logger.LogInformation($"User [{request.UserName}] logged in the system.");

            return Ok(jwtResult);
        }

        [HttpGet("me")]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginResultDto
            {
                UserId =  HttpContext.User.FindFirstValue("Id"),
                UserName = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            // TODO: refresh token should use a minimized version of token
            try
            {
                var userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");


                var accessToken = await HttpContext.GetTokenAsync("Bearer", "accessToken");
                var jwtResult = _jwtAuthManager.Refresh(accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResultDto
                {
                    UserName = userName,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = jwtResult.AccessToken,
                });
            }
            catch (SecurityTokenException e)
            {
                return
                    Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }
    }
}