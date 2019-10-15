using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Squadio.Common.Models.Requests;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next
            //, ILogger<ExceptionMiddleware> logger
            )
        {
            _next = next;
            //_logger = logger;
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex.ToString());
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var error = new ErrorResponse
            {
                Message = exception.Message,
                Code = RequestResult.Fatal.ApiCode.Code,
            };

            var result = JsonConvert.SerializeObject(error);

            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            
            return context.Response.WriteAsync(result);
        }
    }
}