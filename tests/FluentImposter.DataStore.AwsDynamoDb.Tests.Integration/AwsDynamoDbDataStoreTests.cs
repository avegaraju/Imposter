using System;
using System.Linq;
using System.Net.Http;
using System.Text;

using Amazon.DynamoDBv2;

using FluentAssertions;

using FluentImposter.Core.Exceptions;
using FluentImposter.DataStore.AwsDynamoDb.Models;

using Microsoft.Extensions.Configuration;

using ServiceStack;
using ServiceStack.Aws.DynamoDb;
using ServiceStack.Aws.Support;

using Xunit;

namespace FluentImposter.DataStore.AwsDynamoDb.Tests.Integration
{
    public class AwsDynamoDbDataStoreTests
    {
        [Fact(Skip = "This requires an active aws token.")]
        [Trait("Category","DynamoDb")]
        public void CreatesTables_WhenNoTablesExists_CanCreateSchemaInAmazonAwsDynamoDb()
        {
            var configuration = LoadConfiguration();
            var options = configuration.GetAWSOptions();

            var client = options.CreateServiceClient<IAmazonDynamoDB>();
            new AwsDynamoDbDataStore(client);

            var dynamo = new PocoDynamo(client);

            dynamo.GetTableNames()
                      .Any(t => t.Equals(nameof(Sessions)))
                      .Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void CreatesTables_WhenNoTablesExists_CanCreateSchemaInLocalDynamoDbInstance()
        {
            AwsDynamoDbDataStore sut = CreateSut();

            sut.CreateSession();

            var dynamo = new PocoDynamo(GetAmazonDynamoDbClient());

            var tableNames = dynamo.GetTableNames();

            tableNames.Any(t => t.Equals(nameof(Sessions)))
                      .Should().BeTrue();
            tableNames.Any(t => t.Equals(nameof(Requests)))
                      .Should().BeTrue();
            tableNames.Any(t => t.Equals(nameof(Responses)))
                      .Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void CreateSession_CanCreateASession()
        {
            AwsDynamoDbDataStore sut = CreateSut();

            var sessionId = sut.CreateSession();


            var dynamo = new PocoDynamo(GetAmazonDynamoDbClient());

            dynamo.GetItem<Sessions>(sessionId.ToString()).Id
                      .Should().Be(sessionId);
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void EndSession_CanEndASession()
        {
            AwsDynamoDbDataStore sut = CreateSut();

            var sessionId = sut.CreateSession();

            sut.EndSession(sessionId);

            var dynamo = new PocoDynamo(GetAmazonDynamoDbClient());

            var sessionInstance = dynamo.GetItem<Sessions>(sessionId.ToString());

            sessionInstance.EndDateTime
                           .Should().BeCloseTo(DateTime.Now.ToUniversalTime(), 10000);
            sessionInstance.Status
                           .Should().Be(SessionStatus.Completed.ToString());
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void EndSession_WithIncorrectSessionId_ThrowsSessionNotFoundException()
        {
            AwsDynamoDbDataStore sut = CreateSut();

            sut.CreateSession();

            Action exceptionAction = () => sut.EndSession(Guid.NewGuid());

            exceptionAction.Should().Throw<SessionNotFoundException>();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void StoreRequest_WithNonExistingSessionId_ThrowsException()
        {
            Action exceptionThrowingAction
                    = () => CreateSut()
                              .StoreRequest(Guid.NewGuid(), "/", HttpMethod.Post, null);

            exceptionThrowingAction.Should().Throw<SessionNotFoundException>();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void StoreRequest_WithAnAlreadyCompletedSessionId_ThrowsException()
        {
            var sut = CreateSut();

            var sessionId = sut.CreateSession();
            sut.EndSession(sessionId);

            Action exceptionThrowingAction
                    = () => sut
                              .StoreRequest(sessionId, "/", HttpMethod.Post, null);

            exceptionThrowingAction
                .Should().Throw<SessionNoLongerActiveException>();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void StoreRequest_WithAnActiveSessionId_StoresTheRequest()
        {
            var sut = CreateSut();

            var sessionId = sut.CreateSession();

            var requestId = sut.StoreRequest(sessionId,
                                             "/test",
                                             HttpMethod.Post,
                                             "test".ToAsciiBytes());

            var dynamo = new PocoDynamo(GetAmazonDynamoDbClient());

            var request = dynamo.GetItem<Requests>(requestId.ToString());

            request.HttpMethod
                   .Should().Be(HttpMethod.Post.ToString());
            request.Resource
                   .Should().Be("/test");
            request.SessionId
                   .Should().Be(sessionId);
            request.RequestPayloadBase64
                   .Should().Be("test".ToAsciiBytes().ToBase64String());
        }

        private static AwsDynamoDbDataStore CreateSut()
        {
            AmazonDynamoDBClient client = GetAmazonDynamoDbClient();

            AwsDynamoDbDataStore awsDynamoDbDataStore = new AwsDynamoDbDataStore(client);
            return awsDynamoDbDataStore;
        }

        private static AmazonDynamoDBClient GetAmazonDynamoDbClient()
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            };

            var client = new AmazonDynamoDBClient(config);
            return client;
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
