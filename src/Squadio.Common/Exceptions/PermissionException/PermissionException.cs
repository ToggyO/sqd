using System;

namespace Squadio.Common.Exceptions.PermissionException
{
    public class PermissionException: Exception
    {
        public string Code { get; }

        public PermissionException() : this("permission_error")
        {
        }

        public PermissionException(string code) : base("The client is authenticated but doesn't have permissions for the action")
        {
            Code = code;
        }

        public PermissionException(string code, string message) : base(message)
        {
            Code = code;
        }

        public PermissionException(string code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }
}
