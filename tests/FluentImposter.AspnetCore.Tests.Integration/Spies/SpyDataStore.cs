using System;

using FluentImposter.Core;

namespace FluentImposter.AspnetCore.Tests.Integration.Spies
{
    public class SpyDataStore: IDataStore
    {
        public Guid NewSessionId { get; private set; } 
        public bool MockSessionStatusEnded { get; private set; }

        public SpyDataStore()
        {
            NewSessionId = Guid.Empty;
        }

        public Guid CreateSession()
        {
            NewSessionId = Guid.NewGuid();

            return NewSessionId;
        }

        public void EndSession(Guid guid)
        {
            MockSessionStatusEnded = true;
        }
    }
}
