using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Factories;
using Squadio.Common.Enums;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;

namespace Squadio.API.Filters
{
    public class AuthorizationFilter: AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly ITokensFactory _tokenService;
        private readonly ILogger<AuthorizationFilter> _logger;

        public AuthorizationFilter(ITokensFactory tokenService
            , ILogger<AuthorizationFilter> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }
        
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var attributes = descriptor.MethodInfo.CustomAttributes;
                if (attributes.Any(a => a.AttributeType == typeof(AllowAnonymousAttribute)))
                {
                    return;
                }
            }

            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            var tokenStatus = TokenStatus.Invalid;

            if (!string.IsNullOrEmpty(authHeader))
            {
                var token = GetCleanToken(authHeader);
                tokenStatus = await ValidateToken(token);

                if (tokenStatus == TokenStatus.Valid)
                {
                    return;
                }
            }
            var error = GetErrorByTokenStatus(tokenStatus);

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int) error.HttpStatusCode
            };
        }
        

        private ErrorResponse GetErrorByTokenStatus(TokenStatus tokenStatus)
        {
            switch (tokenStatus)
            {
                case TokenStatus.Expired:
                    return new SecurityErrorResponse(new Error
                    {
                        Code = ErrorCodes.Security.AccessTokenExpired,
                        Message = ErrorMessages.Security.AccessTokenExpired
                    });
                case TokenStatus.Invalid:
                    return new SecurityErrorResponse(new Error
                    {
                        Code = ErrorCodes.Security.AccessTokenInvalid,
                        Message = ErrorMessages.Security.AccessTokenInvalid
                    });
            }

            return new SecurityErrorResponse();
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
        private async Task<TokenStatus> ValidateToken(string token)
        {
            return await Task.Run(() => _tokenService.ValidateToken(token, out _));
        }
    }
}