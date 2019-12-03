using System.Collections.Generic;
using System.Net;
using Squadio.Common.Models.Errors;

namespace Squadio.Common.Models.Responses
{
    public class BusinessConflictErrorResponse<T> : ErrorResponse<T> where T : class
    {
        public BusinessConflictErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Global.BusinessConflict;
            Message = ErrorMessages.Global.BusinessConflict;
            HttpStatusCode = HttpStatusCode.Conflict;

            Errors = errors;
        }

        public BusinessConflictErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class BusinessConflictErrorResponse : ErrorResponse
    {
        public BusinessConflictErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Global.BusinessConflict;
            Message = ErrorMessages.Global.BusinessConflict;
            HttpStatusCode = HttpStatusCode.Conflict;

            Errors = errors;
        }

        public BusinessConflictErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class PermissionDeniedErrorResponse<T> : ErrorResponse<T> where T : class
    {
        public PermissionDeniedErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Security.PermissionDenied;
            Message = ErrorMessages.Security.PermissionDenied;
            HttpStatusCode = HttpStatusCode.Forbidden;

            Errors = errors;
        }

        public PermissionDeniedErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class PermissionDeniedErrorResponse : ErrorResponse
    {
        public PermissionDeniedErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Security.PermissionDenied;
            Message = ErrorMessages.Security.PermissionDenied;
            HttpStatusCode = HttpStatusCode.Forbidden;

            Errors = errors;
        }

        public PermissionDeniedErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class SecurityErrorResponse<T> : ErrorResponse<T> where T : class
    {
        public SecurityErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Security.Unauthorized;
            Message = ErrorMessages.Security.Unauthorized;
            HttpStatusCode = HttpStatusCode.Unauthorized;

            Errors = errors;
        }

        public SecurityErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class SecurityErrorResponse : ErrorResponse
    {
        public SecurityErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Security.Unauthorized;
            Message = ErrorMessages.Security.Unauthorized;
            HttpStatusCode = HttpStatusCode.Unauthorized;

            Errors = errors;
        }

        public SecurityErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class InternalErrorResponse : ErrorResponse
    {
        public InternalErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.System.InternalError;
            Message = ErrorMessages.System.InternalError;
            Errors = errors;
        }

        public InternalErrorResponse(Error error) : this(new[] {error})
        {
        }

        public InternalErrorResponse() : this(new List<Error>())
        {
        }
    }
    
    public class AccessTokenInvalidErrorResponse : ErrorResponse
    {
        public AccessTokenInvalidErrorResponse()
        {
            Code = ErrorCodes.Security.Unauthorized;
            Message = ErrorMessages.Security.Unauthorized;
            HttpStatusCode = HttpStatusCode.Unauthorized;

            Errors = new []
            {
                new Error
                {
                    Code = ErrorCodes.Security.AccessTokenInvalid,
                    Message = ErrorMessages.Security.AccessTokenInvalid
                }, 
            };
        }
    }
    
    public class AccessTokenExpiredErrorResponse : ErrorResponse
    {
        public AccessTokenExpiredErrorResponse()
        {
            Code = ErrorCodes.Security.Unauthorized;
            Message = ErrorMessages.Security.Unauthorized;
            HttpStatusCode = HttpStatusCode.Unauthorized;

            Errors = new []
            {
                new Error
                {
                    Code = ErrorCodes.Security.AccessTokenExpired,
                    Message = ErrorMessages.Security.AccessTokenExpired
                }, 
            };
        }
    }
    
    public class RefreshTokenInvalidErrorResponse : ErrorResponse
    {
        public RefreshTokenInvalidErrorResponse()
        {
            Code = ErrorCodes.Security.Unauthorized;
            Message = ErrorMessages.Security.Unauthorized;
            HttpStatusCode = HttpStatusCode.Unauthorized;

            Errors = new []
            {
                new Error
                {
                    Code = ErrorCodes.Security.RefreshTokenInvalid,
                    Message = ErrorMessages.Security.RefreshTokenInvalid
                }, 
            };
        }
    }
    
    public class RefreshTokenExpiredErrorResponse : ErrorResponse
    {
        public RefreshTokenExpiredErrorResponse()
        {
            Code = ErrorCodes.Security.Unauthorized;
            Message = ErrorMessages.Security.Unauthorized;
            HttpStatusCode = HttpStatusCode.Unauthorized;

            Errors = new []
            {
                new Error
                {
                    Code = ErrorCodes.Security.RefreshTokenExpired,
                    Message = ErrorMessages.Security.RefreshTokenExpired
                }, 
            };
        }
    }
}