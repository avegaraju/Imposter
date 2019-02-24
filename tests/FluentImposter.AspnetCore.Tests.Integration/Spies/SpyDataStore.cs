using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;
using FluentImposter.Core.Models;

namespace FluentImposter.AspnetCore.Tests.Integration.Spies
{
    public class SpyDataStore: IDataStore
    {
        private readonly IList<Requests> _requests = new List<Requests>();
        internal IReadOnlyCollection<Requests> Requests => _requests.ToList();

        public Guid NewSessionId { get; private set; } 
        public Guid EndedSessionId { get; private set; }
        public Guid SessionIdReceivedWithRequest { get; private set; }
        public Guid NewRequestId { get; set; }
        public string RequestedResource { get; set; }
        public string HttpMethod { get; set; }
        public byte[] RequestPayload { get; set; }
        public Guid NewResponseId { get; set; }
        public Guid RequestIdReceivedWhileStoringResponse { get; set; }
        public string MatchedCondition { get; set; }
        public string ImposterName { get; set; }
        public byte[] ResponsePayload { get; set; }
        

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
            EndedSessionId = guid;
        }

        public Guid StoreRequest(string resource, HttpMethod method, byte[] requestPayload)
        {
            var requestId = Guid.NewGuid();
            _requests.Add(new Requests
                          {
                              SessionId = sessionId,
                              Id = requestId,
                              Resource = resource,
                              HttpMethod = method.ToString(),
                              RequestPayloadBase64 = Convert.ToBase64String(requestPayload),
                          });

            return requestId;
        }

        public Guid StoreResponse(Guid requestId, string imposterName, string matchedCondition, byte[] responsePayload)
        {
            RequestIdReceivedWhileStoringResponse = requestId;

            MatchedCondition = matchedCondition;
            ImposterName = imposterName;
            ResponsePayload = responsePayload;

            return NewResponseId = Guid.NewGuid();
        }

        public IEnumerable<VerificationResponse> GetVerificationResponse(Guid sessionId, string resource)
        {
            return new List<VerificationResponse>
                   {
                       new VerificationResponse()
                       {
                           Resource = resource
                       }
                   };
        }
    }
}
