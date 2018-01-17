using System;

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

            _dynamo.PutItem<Sessions>(new Sessions()
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

            _dynamo.UpdateItemNonDefaults<Sessions>(new Sessions()
                                                       {
                                                           Id = sessionId,
                                                           EndDateTime = DateTime.Now.ToUniversalTime(),
                                                           Status = "Completed"
                                                       });
        }

        private bool SessionExists(Guid sessionId)
        {
            return _dynamo.GetItem<Sessions>(sessionId.ToString()) != null;
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
