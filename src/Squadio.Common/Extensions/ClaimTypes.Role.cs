using System;
using System.Security.Claims;

namespace Squadio.Common.Extensions
{
    public static partial class ClaimTypes
    {
        public static string RoleId => "role_id";

        public static Guid GetRoleId(this ClaimsPrincipal principal)
        {
            return Guid.TryParse(principal.FindFirst(RoleId)?.Value, out var result) ? result : Guid.Empty;
        }
    }
}
