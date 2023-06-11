using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Internal;

namespace KeylessGateways.Common
{
    /// <summary>
    /// Logs all the incoming api requests with their input models
    /// </summary>
    class AutoLogMiddleWare
    {
        private readonly ILogger<AutoLogMiddleWare> _logger;
         private readonly RequestDelegate _next;

        public AutoLogMiddleWare(RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory?.CreateLogger<AutoLogMiddleWare>() ??
                      throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

            _logger.LogInformation("Incoming request: {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}", context.Request.Scheme, context.Request.Host,context.Request.Path, context.Request.QueryString, body);

            context.Request.Body.Seek(0, SeekOrigin.Begin);

        
            await _next(context);

        }

    }

    public static class AutoLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseAutoLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AutoLogMiddleWare>();
        }
    }
}
