using Autofac.Extensions.DependencyInjection;
using KeylessGateways.Common;
using KeylessGateways.Management.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KeylessGateways.Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Information("Configuring web host ({ApplicationContext})...", AppName);
            var host = CreateHostBuilder(args).Build();

            //Log.Information("Applying migrations ({ApplicationContext})...", AppName);
            host.MigrateDbContext<ManagementDbContext>((context, services) =>
                {
                    var env = services.GetService<IHostEnvironment>();
                    var logger = services.GetService<ILogger<ManagementDbContextSeed>>();

                    new ManagementDbContextSeed()
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