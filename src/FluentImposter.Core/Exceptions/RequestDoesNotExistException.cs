using System;

namespace FluentImposter.Core.Exceptions
{
    public class RequestDoesNotExistException: Exception
    {
        public RequestDoesNotExistException(string message): base(message)
        {
            
        }
    }
}
