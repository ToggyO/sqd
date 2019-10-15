using System;

namespace Squadio.Common.Exceptions.BusinessLogicExceptions
{
    // TODO: Remove and replace everywhere like in ANOVA
    public class BusinessLogicException: Exception
    {
        public string Code { get; }
        public string Field { get; }

        public BusinessLogicException(string code)
        {
            Code = code;
        }

        public BusinessLogicException(string code, string message) : base(message)
        {
            Code = code;
        }

        public BusinessLogicException(string code, string message, string field) : base(message)
        {
            Code = code;
            Field = field;
        }

        public BusinessLogicException(string code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }

        public BusinessLogicException(string code, string message, string field, Exception innerException) : base(message, innerException)
        {
            Code = code;
            Field = field;
        }
    }
}
