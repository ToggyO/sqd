using System.Net;
using Squadio.Common.Models.Responses;

namespace Squadio.Common.Models.Requests
{
    public static class RequestResult
    {
        public static readonly ApiDescription Success = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.OK
            }
        };

        public static readonly ApiDescription RefreshTokenNotExists = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.Unauthorized,
                Code = "refresh_token_invalid"
            },
            Description = "Refresh token doesn't exists or expired",
            Field = "refreshToken"
        };

        public static readonly ApiDescription SetPasswordTokenNotExists = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.NotFound,
                Code = "token_invalid"
            },
            Description = "Token doesn't exists or expired",
            Field = "Token"
        };

        public static readonly ApiDescription ExpirateToken = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.Unauthorized,
                Code = "access_token_invalid"
            },
            Description = "Access token doesn't exist or expired",
            Field = "access_token"
        };

        public static readonly ApiDescription NotFound = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.NotFound,
                Code = "not_found"
            },
            Description = "Not found",
        };

        public static readonly ApiDescription UserNotFound = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.NotFound,
                Code = "not_found"
            },
            Description = "User not found",
            Field = "user"
        };

        public static readonly ApiDescription WrongPassword = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.Forbidden,
                Code = "forbidden"
            },
            Description = "wrong password"
        };

        public static readonly ApiDescription AuthError = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.Forbidden,
                Code = "forbidden"
            },
            Description = "That pair login and password is wrong"
        };

        public static readonly ApiDescription Fatal = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.Conflict,
                Code = "fatal"
            },
        };

        public static readonly ApiDescription ServerError = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.InternalServerError,
                Code = "server error"
            },
            Description = "server error"
        };

        public static readonly ApiDescription UserBlocked = new ApiDescription
        {
            ApiCode = new ApiCode
            {
                HttpCode = (int)HttpStatusCode.Forbidden,
                Code = "forbidden"
            },
            Description = "Your account is blocked. Please, contact administrator"
        };
    }
}