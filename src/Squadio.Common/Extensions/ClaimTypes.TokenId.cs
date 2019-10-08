using System;
using System.Security.Claims;

namespace Squadio.Common.Extensions
{
    public static partial class ClaimTypes
    {
        public static string TokenId => "token_id";

        public static Guid? GetTokenId(this ClaimsPrincipal principal)
        {
            return Guid.TryParse(principal.FindFirst(TokenId)?.Value, out var result) ? result : (Guid?)null;
        }
    }
}
