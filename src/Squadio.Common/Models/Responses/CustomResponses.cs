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

    public class ForbiddenErrorResponse<T> : ErrorResponse<T> where T : class
    {
        public ForbiddenErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Security.Forbidden;
            Message = ErrorMessages.Security.Forbidden;
            HttpStatusCode = HttpStatusCode.Forbidden;

            Errors = errors;
        }

        public ForbiddenErrorResponse(Error error) : this(new[] {error})
        {
        }
    }

    public class ForbiddenErrorResponse : ErrorResponse
    {
        public ForbiddenErrorResponse(IEnumerable<Error> errors)
        {
            Code = ErrorCodes.Security.Forbidden;
            Message = ErrorMessages.Security.Forbidden;
            HttpStatusCode = HttpStatusCode.Forbidden;

            Errors = errors;
        }

        public ForbiddenErrorResponse(Error error) : this(new[] {error})
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
}