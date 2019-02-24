using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using FluentImposter.Core;
using FluentImposter.Core.Entities;
using FluentImposter.Core.Exceptions;
using FluentImposter.Core.Models;

using ServiceStack.Aws.DynamoDb;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public class AwsDynamoDbDataStore: IDataStore
    {
        private readonly PocoDynamo _dynamo;

        public AwsDynamoDbDataStore(IAmazonDynamoDB client)
        {
            _dynamo = new PocoDynamo(client);

            InitializeSchema();
        }

        public Guid StoreRequest(string resource, HttpMethod method, byte[] requestPayload)
        {
            var requestId = Guid.NewGuid();

            if(RequestExists(resource, method, requestPayload))
            {
                var storedRequest = FirstOrDefaultRequest(resource, method, requestPayload);

                _dynamo.UpdateItemNonDefaults(new Requests
                {
                    Id = storedRequest.Id,
                    InvocationCount = storedRequest.InvocationCount + 1
                });

                requestId = storedRequest.Id;
            }
            else
            {
                _dynamo.PutItem(new Requests()
                {
                    HttpMethod = method.ToString(),
                    Id = requestId,
                    Resource = resource,
                    RequestPayloadBase64 = Convert.ToBase64String(requestPayload),
                    InvocationCount = 1
                });
            }

            return requestId;
        }

        private bool RequestExists(string resource, HttpMethod method, byte[] requestPayload)
        {
            return FirstOrDefaultRequest(resource, method, requestPayload) != null;
        }

        private Requests FirstOrDefaultRequest(string resource, HttpMethod method, byte[] requestPayload)
        {
            return _dynamo
                    .GetAll<Requests>()
                    .FirstOrDefault(r => r.Resource.Equals(resource, StringComparison.OrdinalIgnoreCase)
                                         && r.HttpMethod.Equals(method.Method.ToString(),
                                                                StringComparison.OrdinalIgnoreCase)
                                         && r.RequestPayloadBase64
                                             .Equals(Convert.ToBase64String(requestPayload)));
        }

        public Guid StoreResponse(Guid requestId, string imposterName, string matchedCondition, byte[] responsePayload)
        {
            if(!RequestExists(requestId))
                throw new RequestDoesNotExistException($"Request with id {requestId} does not exist.");

            var responseId = Guid.NewGuid();
            _dynamo.PutItem(new Responses()
                            {
                                Id = responseId,
                                ImposterName = imposterName,
                                MatchedCondition = matchedCondition,
                                RequestId = requestId,
                                ResponsePayloadBase64 = Convert.ToBase64String(responsePayload)
                            });

            return responseId;
        }

        public IEnumerable<VerificationResponse> GetVerificationResponse(Guid sessionId, string resource)
        {
            var requests = _dynamo.Scan<Requests>(new ScanRequest()
                                                  {
                                                      TableName = "Requests"
                                                  });

            return requests.Where(r => r.SessionId == sessionId
                                       && r.Resource.Equals(resource, StringComparison.OrdinalIgnoreCase))
                           .Select(v => new VerificationResponse()
                                        {
                                            Resource = v.Resource
                                        });
        }

        public void PurgeData<T>()
        {
            _dynamo.DeleteTable<T>();
            _dynamo.RegisterTable<T>();
            _dynamo.InitSchema();
        }

        private bool RequestExists(Guid requestId)
        {
            return _dynamo.GetItem<Requests>(requestId.ToString()) != null;
        }

        private bool SessionExists(Guid sessionId)
        {
            return _dynamo.GetItem<Sessions>(sessionId.ToString())!=null;
        }

        private bool SessionAlreadyCompleted(Guid sessionId)
        {
            return _dynamo.GetItem<Sessions>(sessionId.ToString())
                          .Status.Equals(SessionStatus.Completed.ToString());
        }

        private void InitializeSchema()
        {
            _dynamo.RegisterTable<Sessions>();
            _dynamo.RegisterTable<Requests>();
            _dynamo.RegisterTable<Responses>();

            _dynamo.InitSchema();
        }
    }

    public enum SessionStatus
    {
        Completed,
        Active
    }
}
