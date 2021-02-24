using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Squadio.Common.Models.Responses;
using Squadio.Common.Enums;
using Squadio.Common.Models.Errors;
using Squadio.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Squadio.Domain.Enums;

namespace Squadio.API.Filters
{
    public class PermissionFilter: Attribute, IAsyncActionFilter
    {
        private readonly Area _area;

        public PermissionFilter(Area area)
        {
            _area = area;
        }

        private PermissionFilter(){}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                var currentFilter = context.ActionDescriptor
                    .FilterDescriptors
                    .Select(filterDescriptor => filterDescriptor)
                    .First(filterDescriptor => ReferenceEquals(filterDescriptor.Filter, this));

                if (currentFilter.Scope != FilterScope.Action)
                {
                    var attributes = descriptor.MethodInfo.CustomAttributes;

                    if (attributes.Any(a =>
                        a.AttributeType == typeof(PermissionFilter) ||
                        a.AttributeType == typeof(AllowAnonymousAttribute)))
                    {
                        await next();
                        return;
                    }
                }
            }

            var role = context.HttpContext.User.GetRoleId();

            if (_area == Area.Admin && role != RoleGuid.Admin || role == Guid.Empty)
            {
                var error = new ErrorResponse
                {
                    Message = ErrorMessages.Security.Forbidden,
                    Code = ErrorCodes.Security.Forbidden
                };

                context.Result = new ObjectResult(error)
                {
                    StatusCode = (int) HttpStatusCode.Forbidden
                };
            }
            else
            {
                await next();
            }
        }
    }
}