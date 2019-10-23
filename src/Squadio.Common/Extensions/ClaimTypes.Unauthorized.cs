using System;
using System.Security.Claims;
using Squadio.Common.Models.Errors;
using Squadio.Common.Models.Responses;

namespace Squadio.Common.Extensions
{
    public static partial class ClaimTypes
    {
        public static ErrorResponse Unauthorized(this ClaimsPrincipal principal)
        {
            return new SecurityErrorResponse(new Error
            {
                Code = ErrorCodes.Security.Unauthorized,
                Field = ErrorFields.User.Token,
                Message = ErrorMessages.Security.Unauthorized
            });
        }
        
        public static ErrorResponse<T> Unauthorized<T>(this ClaimsPrincipal principal) where T : class
        {
            return new SecurityErrorResponse<T>(new Error
            {
                Code = ErrorCodes.Security.Unauthorized,
                Field = ErrorFields.User.Token,
                Message = ErrorMessages.Security.Unauthorized
            });
        }
    }
}
