using System;

namespace FluentImposter.Core.Exceptions
{
    internal class SessionNotFoundException: Exception
    {
        public SessionNotFoundException(string message): base(message)
        {
            
        }
    }
}
