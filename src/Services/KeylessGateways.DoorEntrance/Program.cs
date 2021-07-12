using Autofac.Extensions.DependencyInjection;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.DoorEntrance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Information("Configuring web host ({ApplicationContext})...", AppName);
            var host = CreateHostBuilder(args).Build();

            //Log.Information("Applying migrations ({ApplicationContext})...", AppName);
            host.MigrateDbContext<DoorEntranceDbContext>((context, services) =>
                {
                    var env = services.GetService<IHostEnvironment>();
                    var logger = services.GetService<ILogger<DoorEntranceDbContextSeed>>();

                    new DoorEntranceDbContextSeed()
                        .SeedAsync(context, env, logger)
                        .Wait();
                })
                ;

            //Log.Information("Starting web host ({ApplicationContext})...", AppName);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

    }
}