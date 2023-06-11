using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Identity.Data
{
    public class KGIdentityDbContextSeed
    {
        private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public async Task SeedAsync(KGIdentityDbContext context,
            IServiceProvider services, int? retry = 0)
        {
            var logger = services.GetService<ILogger<KGIdentityDbContextSeed>>();
            var roleManager = services.GetService<RoleManager<Role>>();
            var userManager = services.GetService<UserManager<User>>();


            int retryForAvailability = retry ?? 0;

            try
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new Role {Name = "Admin",});
                }

                if (!context.Users.Any())
                {
                    var user = new User
                    {
                        UserName = "admin@kg.com",
                        Email = "admin@kg.com",
                        SecurityStamp = Guid.NewGuid().ToString("D"),
                    };
                    var res = await userManager.CreateAsync(user, "Pass@word1");
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(KGIdentityDbContext));

                    await SeedAsync(context, services, retryForAvailability);
                }
            }
        }
    }
}