using System;
using System.Text;
using Autofac;
using KeylessGateways.Common;
using KeylessGateways.Identity.Data;
using KeylessGateways.Identity.Infrastructure;
using KeylessGateways.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace KeylessGateways.Identity
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
            builder.RegisterType<KGIdentityDbContext>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            builder.RegisterType<KGIdentityDbContext>();
            builder.RegisterType<JwtAuthManager>().As<IJwtAuthManager>();
            builder.RegisterType<UserService>().As<IUserService>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutoMapper(typeof(Startup))
                .AddOptions()
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
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "KeylessGateways.Identity v1");
                c.DocumentTitle = "KeylessGateways.Identity";
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
            services.AddDbContext<DbContext, KGIdentityDbContext>(options =>
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
            //services.AddHealthChecks()
            //    .AddCheck("self", () => HealthCheckResult.Healthy())
            //    .AddSqlServer(Configuration["ConnectionString"],
            //        name: "IdentityDB-check",
            //        tags: new string[] {"IdentityDB"});
            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<KGIdentityDbContext>()
                .AddDefaultTokenProviders();


            var jwtTokenConfig = configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            });
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "KeylessGateways.Identity", Version = "v1"});

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
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
    }
}