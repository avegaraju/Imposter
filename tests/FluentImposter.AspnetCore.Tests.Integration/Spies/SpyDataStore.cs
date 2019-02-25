using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using FluentImposter.Core;
using FluentImposter.Core.Entities;
using FluentImposter.Core.Models;

namespace FluentImposter.AspnetCore.Tests.Integration.Spies
{
    public class SpyDataStore: IDataStore
    {
        private IList<Requests> _requests = new List<Requests>();
        private IList<Responses> _responses = new List<Responses>();

        internal IReadOnlyCollection<Requests> Requests => _requests.ToList();
        internal IReadOnlyCollection<Responses> Responses => _responses.ToList();

        public Guid NewRequestId { get; set; }
        public string RequestedResource { get; set; }
        public string HttpMethod { get; set; }
        public byte[] RequestPayload { get; set; }
        public Guid NewResponseId { get; set; }
        public Guid RequestIdReceivedWhileStoringResponse { get; set; }
        public string MatchedCondition { get; set; }
        public string ImposterName { get; set; }
        public byte[] ResponsePayload { get; set; }

        public Guid StoreRequest(string resource, HttpMethod method, byte[] requestPayload)
        {
            var requestId = Guid.NewGuid();
            _requests.Add(new Requests
                          {
                              Id = requestId,
                              Resource = resource,
                              HttpMethod = method.ToString(),
                              RequestPayloadBase64 = Convert.ToBase64String(requestPayload),
                          });

            return requestId;
        }

        public void StoreResponse(Guid requestId,
                                  string imposterName,
                                  string matchedCondition,
                                  byte[] responsePayload)
        {
            _responses.Add(new Responses
                           {
                               RequestId = requestId,
                               ResponsePayloadBase64 = Convert.ToBase64String(responsePayload),
                               ImposterName = imposterName,
                               MatchedCondition = matchedCondition
                           });
        }

        public VerificationResponse GetVerificationResponse(string resource, HttpMethod method, byte[] requestPayload)
        {
            var storedRequest = Requests.First(r => r.Resource.Equals(resource, StringComparison.OrdinalIgnoreCase)
                                && r.HttpMethod.Equals(method.Method.ToString(), StringComparison.OrdinalIgnoreCase)
                                && r.RequestPayloadBase64.Equals(Convert.ToBase64String(requestPayload)));

            return new VerificationResponse()
                   {
                       Resource = resource,
                       RequestPayload = Encoding.ASCII.GetString(requestPayload),
                       InvocationCount = storedRequest.InvocationCount
                   };
        }

        public void PurgeData<T>()
        {
            if (typeof (T) == typeof(Requests))
            {
                _requests = new List<Requests>();
            }
        }
    }
}
