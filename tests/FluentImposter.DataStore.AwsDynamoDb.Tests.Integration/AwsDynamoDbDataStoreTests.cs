using System;
using System.Linq;

using Amazon.DynamoDBv2;

using FluentAssertions;

using FluentImposter.Core.Exceptions;
using FluentImposter.DataStore.AwsDynamoDb.Models;

using Microsoft.Extensions.Configuration;

using ServiceStack.Aws.DynamoDb;

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

            var dynamo = new PocoDynamo(client);

            dynamo.GetTableNames()
                      .Any(t => t.Equals(nameof(FI_SESSIONS)))
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

            var dynamo = new PocoDynamo(client);

            dynamo.GetTableNames()
                      .Any(t => t.Equals(nameof(FI_SESSIONS)))
                      .Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void CreateSession_CanCreateASession()
        {
            var config = new AmazonDynamoDBConfig
                         {
                             ServiceURL = "http://localhost:8000"
                         };

            var client = new AmazonDynamoDBClient(config);

            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);

            var sessionId = awsDynamoDbDataStore.CreateSession();

            var dynamo = new PocoDynamo(client);

            dynamo.GetItem<FI_SESSIONS>(sessionId.ToString()).Id
                      .Should().Be(sessionId);
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void EndSession_CanEndASession()
        {
            var config = new AmazonDynamoDBConfig
                         {
                             ServiceURL = "http://localhost:8000"
                         };

            var client = new AmazonDynamoDBClient(config);

            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);

            var sessionId = awsDynamoDbDataStore.CreateSession();

            awsDynamoDbDataStore.EndSession(sessionId);

            var dynamo = new PocoDynamo(client);

            var sessionInstance = dynamo.GetItem<FI_SESSIONS>(sessionId.ToString());

            sessionInstance.EndDateTime
                           .Should().BeCloseTo(DateTime.Now.ToUniversalTime(), 10000);
            sessionInstance.Status
                           .Should().Be("Completed");
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void EndSession_WithIncorrectSessionId_ThrowsSessionNotFoundException()
        {
            var config = new AmazonDynamoDBConfig
                         {
                             ServiceURL = "http://localhost:8000"
                         };

            var client = new AmazonDynamoDBClient(config);

            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);

            var sessionId = awsDynamoDbDataStore.CreateSession();

            Action exceptionAction = () => awsDynamoDbDataStore.EndSession(Guid.NewGuid());

            exceptionAction.Should().Throw<SessionNotFoundException>();
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
