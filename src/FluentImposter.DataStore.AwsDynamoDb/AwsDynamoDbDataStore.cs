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
        private readonly AmazonDynamoDBClient _amazonDynamoDbClient;

        public AwsDynamoDbDataStore()
        {
            _amazonDynamoDbClient = new AmazonDynamoDBClient();

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
            var putItemResponse  = _amazonDynamoDbClient.PutItemAsync(putItemRequest).Result;

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
            return _amazonDynamoDbClient.ListTablesAsync(new ListTablesRequest())
                                 .Result.TableNames.Any(t => t.Equals("FI_SESSION"));
        }
    }
}
