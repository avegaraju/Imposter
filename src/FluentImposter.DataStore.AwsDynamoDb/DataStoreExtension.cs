using Amazon.DynamoDBv2;

using FluentImposter.Core;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public static class DataStoreExtension
    {
        public static ImpostersAsMockConfiguration UseDynamoDb(this ImpostersAsMockConfiguration impostersConfiguration,
                                                        IAmazonDynamoDB amazonDynamoDbClient)
        {
            impostersConfiguration.SetDataStore(new AwsDynamoDbDataStore(amazonDynamoDbClient));

            return impostersConfiguration;
        }
    }
}
