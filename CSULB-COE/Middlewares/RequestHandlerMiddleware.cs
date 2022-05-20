using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSULB_COE.Middlewares
{
    
    public class RequestHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestHandlerMiddleware(ILogger<RequestHandlerMiddleware> logger, RequestDelegate next)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            StringBuilder sbLogTrace = new StringBuilder();
            //_logger.LogTrace($"Header: {JsonConvert.SerializeObject(context.Request.Headers, Formatting.Indented)}");
            sbLogTrace.Append($"Header: {JsonConvert.SerializeObject(context.Request.Headers, Formatting.Indented)}");

            context.Request.EnableBuffering();
            var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
            //_logger.LogTrace($"Body: {body}");
            sbLogTrace.Append(" " + $"Body: {body}");
            context.Request.Body.Position = 0;

            // _logger.LogTrace($"Host: {context.Request.Host.Host}");
            sbLogTrace.Append(" " + $"Host: {context.Request.Host.Host}");
            // _logger.LogTrace($"Client IP: {context.Connection.RemoteIpAddress}");
            sbLogTrace.Append(" "+ $"Client IP: {context.Connection.RemoteIpAddress}");
            _logger.LogTrace(sbLogTrace.ToString());
            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestHandlerMiddleware>();
        }
    }
}
