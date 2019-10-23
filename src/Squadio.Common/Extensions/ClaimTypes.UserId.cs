using System;
using System.Security.Claims;

namespace Squadio.Common.Extensions
{
    public static partial class ClaimTypes
    {
        public static string UserId => "user_id";

        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            return Guid.TryParse(principal.FindFirst(UserId)?.Value, out var result) ? result : (Guid?) null;
        }
    }
}
