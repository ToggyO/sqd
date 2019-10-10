using System.Collections.Generic;
using System.Net;

namespace Squadio.Common.Models.Responses
{
    public class Response
    {
        public string Code { get; set; } = "success";
    }
    public class Response<T> : Response where T : class
    {
        public T Data { get; set; }
    }
    public class ErrorResponse : Response
    {
        public string Message { get; set; }

        public IEnumerable<Error> Errors { get; set; } = new Error[0];
    }
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Field { get; set; }
    }
    public class ErrorResponse<T> : Response<T> where T : class
    {
        public string Message { get; set; }

        public IEnumerable<Error> Errors { get; set; } = new Error[0];
    }
}
