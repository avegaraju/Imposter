using System;

namespace FluentImposter.Core.Exceptions
{
    public class InvalidSessionIdInUriException: Exception
    {
        public InvalidSessionIdInUriException(string message):base(message)
        {
        }
    }
}
