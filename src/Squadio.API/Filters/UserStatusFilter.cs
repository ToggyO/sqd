using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Squadio.BLL.Factories;
using Squadio.BLL.Providers.Users;
using Squadio.Common.Enums;
using Squadio.Common.Extensions;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;
using Squadio.Domain.Enums;

namespace Squadio.API.Filters
{
    public class UserStatusFilter: AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly IUsersProvider _usersProvider;
        private readonly ILogger<UserStatusFilter> _logger;

        public UserStatusFilter(IUsersProvider usersProvider
            , ILogger<UserStatusFilter> logger)
        {
            _usersProvider = usersProvider;
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

            var claimsPrincipal = context.HttpContext.User;
            var userStatus = await GetUserStatus(claimsPrincipal);
            
            if (userStatus != UserStatus.Blocked)
            {
                return;
            }

            var error = new ForbiddenErrorResponse(new Error
            {
                Code = ErrorCodes.Security.UserBlocked,
                Message = ErrorMessages.Security.UserBlocked
            });

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int) error.HttpStatusCode
            };
        }

        private async Task<UserStatus> GetUserStatus(ClaimsPrincipal claims)
        {
            var userId = claims.GetUserId();
            var userResponse = await _usersProvider.GetById(userId);
            if (userResponse.IsSuccess)
            {
                if (userResponse.Data != null)
                {
                    return userResponse.Data.Status;
                }
            }
            return UserStatus.Blocked;
        }
    }
}