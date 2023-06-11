using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance.Data
{
    public class DoorEntranceDbContextSeed
    {
        public async Task SeedAsync(DoorEntranceDbContext context, IHostEnvironment env,
            ILogger<DoorEntranceDbContextSeed> logger, int retry = 0)
        {
            int retryForAvailability = retry ;

            try
            {
                // insert possible seed
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(DoorEntranceDbContext));

                    await SeedAsync(context, env, logger, retryForAvailability);
                }
            }
        }

    }
}