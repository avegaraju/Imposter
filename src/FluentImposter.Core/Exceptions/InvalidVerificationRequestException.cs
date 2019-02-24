using System;

namespace FluentImposter.Core.Exceptions
{
    public class InvalidVerificationRequestException: Exception
    {
        public InvalidVerificationRequestException(string message)
            :base(message)
        {
        }
        public InvalidVerificationRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
