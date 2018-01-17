using System;
using System.Net.Http;

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

        public Guid StoreRequest(Guid sessionId, string resource, HttpMethod method, byte[] requestPayload)
        {
            throw new NotImplementedException();
        }

        public Guid StoreResponse(Guid requestId, string imposterName, string matchedCondition, byte[] responsePayload)
        {
            throw new NotImplementedException();
        }
    }
}
