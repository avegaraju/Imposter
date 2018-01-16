using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using FluentImposter.Core;

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

            PutItemRequest putItemRequest = new PutItemRequest("FI_SESSION",
                                                               new Dictionary<string, AttributeValue>()
                                                               {
                                                                   {
                                                                       "SESSIONID",
                                                                       new AttributeValue(sessionId)
                                                                   }
                                                               });
            var putItemResponse  = _client.PutItemAsync(putItemRequest).Result;

            return Guid.Parse(sessionId);
        }

        public void EndSession(Guid guid)
        {
            throw new NotImplementedException();
        }

        private void CreateTables()
        {
            if (!TablesExists())
            {
                
            }
        }

        private bool TablesExists()
        {
            return _client.ListTablesAsync(new ListTablesRequest())
                                 .Result.TableNames.Any(t => t.Equals("FI_SESSION"));
        }
    }
}
