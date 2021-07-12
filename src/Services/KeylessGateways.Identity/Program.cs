using Autofac.Extensions.DependencyInjection;
using KeylessGateways.Common;
using KeylessGateways.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace KeylessGateways.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Information("Configuring web host ({ApplicationContext})...", AppName);
            var host = CreateHostBuilder(args).Build();

            //Log.Information("Applying migrations ({ApplicationContext})...", AppName);
            host.MigrateDbContext<KGIdentityDbContext>((context, services) =>
                {
                    new KGIdentityDbContextSeed()
                        .SeedAsync(context, services)
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