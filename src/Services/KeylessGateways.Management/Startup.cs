using System;
using System.Text;
using Autofac;
using KeylessGateways.Common;
using KeylessGateways.Management.Data;
using KeylessGateways.Services.Shared.EventBus;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace KeylessGateways.Management
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<ManagementDbContext>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions()
                .AddAutoMapper(typeof(Startup))
                .AddCustomMassTransit(Configuration)
                .AddHealth()
                .AddDatabase(Configuration)
                .AddAuth(Configuration)
                .AddSwagger()
                .AddCors()
                .AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseAutoLogging();
            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "KeylessGateways.Management v1");
                c.DocumentTitle = "KeylessGateways.Management";
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");
            app.UseMiddleware<HttpStatusExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    internal static class ServiceExtensionsMethods
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<DbContext, ManagementDbContext>(options =>
                {
                    options.UseMySql(configuration["ConnectionString"],
                        new MariaDbServerVersion ("10.5.11"),
                        action =>
                        {
                            action.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        }
                    );
                }
            );

            return services;
        }


        public static IServiceCollection AddHealth(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateActor = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["jwtTokenConfig:secret"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "KeylessGateways.Management", Version = "v1"});
            });
            return services;
        }

        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
            return services;
        } 
        
        public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
          
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    //cfg.UseHealthCheck(provider);
                 
                    cfg.Host(new Uri(configuration["RabbitMQ:Uri"]), h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });
                }));
            });

            services.AddMassTransitHostedService();


            return services;
        }
    }
}