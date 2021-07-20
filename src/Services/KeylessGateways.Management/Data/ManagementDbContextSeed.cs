using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Management.Data
{
    public class ManagementDbContextSeed
    {
        public async Task SeedAsync(ManagementDbContext context, IHostEnvironment env,
            ILogger<ManagementDbContextSeed> logger, int retry = 0)
        {
            int retryForAvailability = retry;

            try
            {
                if (!context.Doors.Any())
                {
                    context.Doors.AddRange(new[]
                    {
                        new Door
                        {
                            Id = new Guid(),
                            Name = "Tunnel",
                        },
                        new Door
                        {
                            Id = new Guid(),
                            Name = "Office",
                        }
                    });

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ManagementDbContext));

                    await SeedAsync(context, env, logger, retryForAvailability);
                }
            }
        }

    }
}