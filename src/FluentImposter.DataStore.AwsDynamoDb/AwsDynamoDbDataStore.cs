using System;
using System.Collections.Generic;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using FluentImposter.Core;
using FluentImposter.DataStore.AwsDynamoDb.Models;

using ServiceStack.Aws.DynamoDb;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public class AwsDynamoDbDataStore: IDataStore
    {
        private readonly IAmazonDynamoDB _client;

        public AwsDynamoDbDataStore(IAmazonDynamoDB client)
        {
            _client = client;

            CreateTables();
        }

        public Guid CreateSession()
        {
            var sessionId = Guid.NewGuid().ToString();

            return Guid.Parse(sessionId);
        }

        public void EndSession(Guid guid)
        {
            throw new NotImplementedException();
        }

        private void CreateTables()
        {
            var database = new PocoDynamo(_client)
                    .RegisterTable<FI_SESSIONS>();

            database.InitSchema();
        }
    }
}
