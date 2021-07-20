using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KeylessGateways.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Identity.Services
{
    public interface IUserService
    {
        Task<(bool Valid, Guid Id)> IsValidUserCredentialsAsync(string userName, string password);
        Task<bool> IsAnExistingUserAsync(string userName);
        Task<IList<string>> GetUserRoleAsync(string userName);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;

        public UserService(ILogger<UserService> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<bool> IsAnExistingUserAsync(string userName)
        {
            var userToVerify = await _userManager.FindByEmailAsync(userName);
            return userToVerify != null;
        }

        public async Task<IList<string>> GetUserRoleAsync(string userName)
        {
            var userToVerify = await _userManager.FindByEmailAsync(userName);
            if (userToVerify == null)
            {
                throw new Exception();
            }

            return await _userManager.GetRolesAsync(userToVerify);
        }

        public async Task<(bool Valid, Guid Id)> IsValidUserCredentialsAsync(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]");
            if (string.IsNullOrWhiteSpace(userName))
            {
                return (false, Guid.Empty);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, Guid.Empty);
            }

            var userToVerify = await _userManager.FindByEmailAsync(userName);
            if (userToVerify == null)
            {
                return (false, Guid.Empty);
            }

            var passwordMatch = await _userManager.CheckPasswordAsync(userToVerify, password);
            if (passwordMatch)
            {
                return (true, userToVerify.Id);
            }

            return (false, Guid.Empty);
        }

    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string BasicUser = nameof(BasicUser);
    }
}