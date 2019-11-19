using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Squadio.BLL.Factories;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Filters
{
    public class AuthorizationFilter: AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private ITokensFactory _service;
        
        /// <inheritdoc />
        public AuthorizationFilter()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            _service = (ITokensFactory) context.HttpContext.RequestServices.GetRequiredService(typeof(ITokensFactory));
            
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var attributes = descriptor.MethodInfo.CustomAttributes;
                if (attributes.Any(a => a.AttributeType == typeof(AllowAnonymousAttribute)))
                {
                    return;
                }
            }

            string authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                var token = GetCleanToken(authHeader);

                if (await ValidateToken(token))
                {
                    return;
                }
            }

            var error = new ErrorResponse
            {
                Message = ErrorMessages.Security.Unauthorized,
                Code = ErrorCodes.Security.Unauthorized,
                Errors = new List<Error>
                {
                    new Error
                    {
                        Message = ErrorMessages.Security.AccessTokenInvalid,
                        Code = ErrorCodes.Security.AccessTokenInvalid
                    }
                }
            };

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int) HttpStatusCode.Unauthorized
            };
        }

        /// <summary>
        /// GetCleanToken
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        private static string GetCleanToken(string authHeader)
        {
            var index = authHeader.IndexOf("Bearer ", StringComparison.CurrentCultureIgnoreCase);
            return index < 0 ? authHeader : authHeader.Remove(index, "Bearer ".Length);
        }

        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<bool> ValidateToken(string token)
        {
            return await Task.Run(() => _service.ValidateToken(token, out _));
        }
    }
}