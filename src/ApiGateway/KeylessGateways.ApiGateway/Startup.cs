using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace KeylessGateways.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync(
@"
<!DOCTYPE html>
<html>
<title>KeylessGateways</title>
<head>
<body>
<div>
  <h1>Welcome to the KeylessGateways application!</h1>
  <p>
  Execute your REST calls against this endpoint (http://localhost:45000). Sample Postman collection including all APIs has been attached to the source repository.
  </p>
  <br/>
  <p>
  Alternatively you click on the below links to browse the individual microservices APIs via Swagger (Exposed only for development purposes):
  </p>
    <p>
  		<ul>
          <li> Identity:<a href=""http://localhost:45010""> http://localhost:45010</a></li>
          <li> Management: <a href=""http://localhost:45011"">http://localhost:45011</a></li>
          <li> Door Entrance: <a href=""http://localhost:45012""> http://localhost:45012</a></li>
	</ul>
  </p>
</div>
</body>
</html> 
"); });
            });

            // For development porposes, as well as Cloud deployment when the API is behind reliable WAF/Gateway
            app.UseCors("AllowAll");
            await app.UseOcelot();
        }
    }
}