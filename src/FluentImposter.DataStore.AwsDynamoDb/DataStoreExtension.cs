using FluentImposter.Core;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public static class DataStoreExtension
    {
        public static ImposterConfiguration UseDynamoDb(this ImposterConfiguration imposterConfiguration)
        {
            imposterConfiguration.SetDataStore(new AwsDynamoDbDataStore());

            return imposterConfiguration;
        }
    }
}
