using Autofac.Extensions.DependencyInjection;
using KeylessGateways.Common;
using KeylessGateways.DoorEntrance.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace KeylessGateways.DoorEntrance
{
    public class Program
    {
        public static void Main(string[] args)
        {
       
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web host");

                Log.Information("Configuring web host...");
                var host = CreateHostBuilder(args).Build();

                Log.Information("Applying migrations...");
                host.MigrateDbContext<DoorEntranceDbContext>((context, services) =>
                    {
                        var env = services.GetService<IHostEnvironment>();
                        var logger = services.GetService<ILogger<DoorEntranceDbContextSeed>>();

                        new DoorEntranceDbContextSeed()
                            .SeedAsync(context, env, logger)
                            .Wait();
                    })
                    ;

                Log.Information("Starting web host...");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
         
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.WithProperty("ApplicationContext", "DoorEntrance")
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.Seq(context.Configuration["Serilog:SeqServerUrl"])
                )
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

    }
}