using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Squadio.Common.Models.Responses;
using System.Threading.Tasks;

namespace Squadio.API.Extensions
{
    internal class BaseErrorsMiddleware
    {
        private readonly RequestDelegate _next;

        public BaseErrorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

                if (context.Request.Path.StartsWithSegments("/api"))
                    switch (context.Response.StatusCode)
                    {
                        case 401:
                            await Handle(context, UnauthorizedContent);
                            return;
                        case 403:
                            await Handle(context, ForbiddenContent);
                            return;
                        case 404:
                            await Handle(context, NotFoundContent);
                            return;
                    }
            }
            catch (Exception e)
            {
                await Handle(context, new ErrorResponse()
                {
                    Code = "unexpected_error",
                    Message = e.Message
                });
            }
        }

        private static ErrorResponse NotFoundContent => new ErrorResponse
        {
            Code = "not_found",
            Message = "Resource is not found",
            Errors = new[]
            {
                new Error
                {
                    Code = "not_found",
                    Message = "Resource is not found"
                }
            }
        };

        private static ErrorResponse ForbiddenContent => new ErrorResponse
        {
            Code = "permission_error",
            Message = "The client is authenticated but doesn't have permissions for the action",
            Errors = new[]
            {
                new Error
                {
                    Code = "permission_error",
                    Message = "The client is authenticated but doesn't have permissions for the action"
                }
            }
        };

        private static ErrorResponse UnauthorizedContent => new ErrorResponse
        {
            Code = "security_error",
            Message = "Identify is required",
            Errors = new[]
            {
                new Error
                {
                    Code = "sec.access_token_invalid",
                    Message = "Access token isn't invalid"
                }
            }
        };

        private static Task Handle(HttpContext context, ErrorResponse content)
        {
            return Handle(context, content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private static async Task Handle(HttpContext context, ErrorResponse content, JsonSerializerSettings serializerSettings)
        {
            if (context.Response.HasStarted) return;

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(content, serializerSettings));
        }
    }
}
