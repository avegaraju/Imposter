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
                                Status = "Active",
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
                                              Status = "Completed"
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
                                RequestPayload = requestPayload
                            });

            return requestId;
        }

        public Guid StoreResponse(Guid requestId, string imposterName, string matchedCondition, byte[] responsePayload)
        {
            throw new NotImplementedException();
        }

        private bool SessionExists(Guid sessionId)
        {
            return _dynamo.GetItem<Sessions>(sessionId.ToString())!=null;
        }

        private bool SessionAlreadyCompleted(Guid sessionId)
        {
            return _dynamo.GetItem<Sessions>(sessionId.ToString())
                          .Status.Equals("Completed");
        }

        private void InitializeSchema()
        {
            _dynamo.RegisterTable<Sessions>();
            _dynamo.RegisterTable<Requests>();
            _dynamo.RegisterTable<Responses>();

            _dynamo.InitSchema();
        }
    }
}
