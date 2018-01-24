using System;
using System.Net.Http;

namespace FluentImposter.Core
{
    internal class NullDataStore: IDataStore
    {
        public Guid CreateSession()
        {
            return Guid.Empty;
        }

        public void EndSession(Guid guid)
        {
        }

        public Guid StoreRequest(Guid sessionId, string resource, HttpMethod method, byte[] requestPayload)
        {
            return Guid.Empty;
        }

        public Guid StoreResponse(Guid requestId, string imposterName, string matchedCondition, byte[] responsePayload)
        {
            return Guid.Empty;
        }
    }
}
