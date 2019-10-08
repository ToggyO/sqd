using System;

namespace Squadio.Common.Exceptions.SecurityExceptions
{
    public class SecurityException: Exception
    {
        public string Code { get; }

        public SecurityException() : this("security_error")
        {
        }

        public SecurityException(string code) : base("Authentication is required")
        {
            Code = code;
        }

        public SecurityException(string code, string message) : base(message)
        {
            Code = code;
        }

        public SecurityException(string code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }
}
