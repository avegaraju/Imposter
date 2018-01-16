using System;
using System.Linq;

using Amazon.DynamoDBv2;

using FluentAssertions;

using Microsoft.Extensions.Configuration;

using Xunit;

namespace FluentImposter.DataStore.AwsDynamoDb.Tests.Integration
{
    public class AwsDynamoDbDataStoreTests
    {
        [Fact]
        [Trait("Category","DynamoDb")]
        public void CreatesTables_WhenNoTablesExists_CanCreateSchemaInAmazonAwsDynamoDb()
        {
            var configuration = LoadConfiguration();
            var options = configuration.GetAWSOptions();

            var client = options.CreateServiceClient<IAmazonDynamoDB>();


            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);

            client.ListTablesAsync()
                  .Result.TableNames.Any(t => t.Equals("FI_SESSIONS"))
                  .Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void CreatesTables_WhenNoTablesExists_CanCreateSchemaInLocalDynamoDbInstance()
        {
            var config = new AmazonDynamoDBConfig
                         {
                             ServiceURL = "http://localhost:8000"
                         };

            var client = new AmazonDynamoDBClient(config);

            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);

            client.ListTablesAsync()
                  .Result.TableNames.Any(t => t.Equals("FI_SESSIONS"))
                  .Should().BeTrue();
        }


        private IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}
