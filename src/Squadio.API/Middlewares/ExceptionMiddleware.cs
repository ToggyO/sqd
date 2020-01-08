using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next
            , ILogger<ExceptionMiddleware> logger
            )
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.Request.Path;
                
                if (path.StartsWithSegments("/api/ws"))
                {
                    _logger.LogInformation("----- ExceptionMiddleware -----");
                    _logger.LogInformation("This request is /api/ws");
                    _logger.LogInformation($"Request path is '{path}'");
                }
                else
                {
                    _logger.LogInformation("This request is ! NOT ! /api/ws");
                    _logger.LogInformation($"Request path is '{path}'");
                }
                
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                // TODO: In production should be this code line ↓
                //_logger.LogError(ex.Message);
                // TODO: In production this code line should be removed ↓
                _logger.LogError(ex, "["+context.Request.Path + context.Request.QueryString+"] " + ex.Message);
                //_logger.LogError(ex, ex.Message);
                
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var error = new InternalErrorResponse(new Error
            {
                // TODO: In production should be this code line ↓
                // Code = ErrorCodes.System.InternalError,
                // TODO: In production this code line should be removed ↓
                Code = exception.StackTrace,
                Message = exception.Message,
            });

            var result = JsonConvert.SerializeObject(error);

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            
            return context.Response.WriteAsync(result);
        }
    }
}