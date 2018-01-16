using Amazon.DynamoDBv2;

using FluentImposter.Core;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public static class DataStoreExtension
    {
        public static ImposterConfiguration UseDynamoDb(this ImposterConfiguration imposterConfiguration,
                                                        IAmazonDynamoDB amazonDynamoDbClient)
        {
            imposterConfiguration.SetDataStore(new AwsDynamoDbDataStore(amazonDynamoDbClient));

            return imposterConfiguration;
        }
    }
}
