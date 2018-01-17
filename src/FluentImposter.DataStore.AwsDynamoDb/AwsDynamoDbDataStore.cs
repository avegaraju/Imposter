using System;
using System.Net.Http;

using Amazon.DynamoDBv2;

using FluentImposter.Core;
using FluentImposter.Core.Exceptions;
using FluentImposter.DataStore.AwsDynamoDb.Models;

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

        public Guid CreateSession()
        {
            var sessionId = Guid.NewGuid();

            _dynamo.PutItem(new Sessions()
                            {
                                StartDateTime = DateTime.Now.ToUniversalTime(),
                                Status = SessionStatus.Active.ToString(),
                                Id = sessionId
                            });

            return sessionId;
        }

        public void EndSession(Guid sessionId)
        {
            if (!SessionExists(sessionId))
                throw new SessionNotFoundException($"No session found with id {sessionId}");

            _dynamo.UpdateItemNonDefaults(new Sessions()
                                          {
                                              Id = sessionId,
                                              EndDateTime = DateTime.Now.ToUniversalTime(),
                                              Status = SessionStatus.Completed.ToString()
                                          });
        }

        public Guid StoreRequest(Guid sessionId,
                                 string resource,
                                 HttpMethod method,
                                 byte[] requestPayload)
        {
            if (!SessionExists(sessionId))
                throw new SessionNotFoundException($"No session found with id {sessionId}");

            if(SessionAlreadyCompleted(sessionId))
                throw new SessionNoLongerActiveException($"The session with id {sessionId} is no longer active.");

            var requestId = Guid.NewGuid();

            _dynamo.PutItem(new Requests()
                            {
                                HttpMethod = method.ToString(),
                                Id = requestId,
                                Resource = resource,
                                RequestPayloadBase64 = Convert.ToBase64String(requestPayload),
                                SessionId = sessionId
                            });

            return requestId;
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
