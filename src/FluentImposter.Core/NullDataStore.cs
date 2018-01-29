using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using FluentImposter.Core.Entities;

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

        public IEnumerable<VerificationResponse> GetVerificationResponse(Guid sessionId, string resource)
        {
            return new List<VerificationResponse>();
        }
    }
}
