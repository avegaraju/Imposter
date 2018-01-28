using System;
using System.Net.Http;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IDataStore
    {
        Guid CreateSession();
        void EndSession(Guid guid);

        Guid StoreRequest(Guid sessionId,
                                string resource,
                                HttpMethod method,
                                byte[] requestPayload);

        Guid StoreResponse(Guid requestId,
                               string imposterName,
                               string matchedCondition,
                               byte[] responsePayload);

        VerificationResponse GetVerificationResponse(Guid sessionId, string resource);
    }
}
