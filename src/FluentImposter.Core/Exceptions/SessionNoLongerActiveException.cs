using System;

namespace FluentImposter.Core.Exceptions
{
    public class SessionNoLongerActiveException: Exception
    {
        public SessionNoLongerActiveException(string message): base(message)
        {
            
        }
    }
}
