using System;
using System.Net.Http;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IDataStore
    {
        Guid StoreRequest(string resource, HttpMethod method, byte[] requestPayload);

        Guid StoreResponse(Guid requestId,
                               string imposterName,
                               string matchedCondition,
                               byte[] responsePayload);

        VerificationResponse GetVerificationResponse(string resource, HttpMethod method, byte[] requestPayload);

        void PurgeData<T>();
    }
}
