using Amazon.DynamoDBv2;

using FluentImposter.DataStore.AwsDynamoDb;

namespace ImpostersServiceSample.Mocks
{
    public class MocksDataStore
    {
        public AwsDynamoDbDataStore Create()
        {
            AmazonDynamoDBClient client = CreateAmazonDynamoDbClientWithLocalDbInstance();

            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);
            return awsDynamoDbDataStore;
        }

        private static AmazonDynamoDBClient CreateAmazonDynamoDbClientWithLocalDbInstance()
        {
            var config = new AmazonDynamoDBConfig
                         {
                             ServiceURL = "http://localhost:8000"
                         };

            return new AmazonDynamoDBClient(config);
        }
    }
}
