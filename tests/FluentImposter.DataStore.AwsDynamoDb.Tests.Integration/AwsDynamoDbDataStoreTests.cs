using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

using Amazon.DynamoDBv2;

using FluentAssertions;

using FluentImposter.Core.Entities;
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

            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());

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


            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());

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

            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());

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

            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());

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

        [Fact]
        public void StoreResponse_WithIncorrectRequestId_ThrowsException()
        {
            var sut = CreateSut();

            Action exceptionThrowingAction
                    = () => sut.StoreResponse(Guid.NewGuid(), "test", "condition expression", null);

            exceptionThrowingAction
                    .Should().Throw<RequestDoesNotExistException>();
        }

        [Fact]
        public void StoreResponse_WithValidRequestId_StoresResponse()
        {
            var sut = CreateSut();

            var sessionId = sut.CreateSession();
            var requestId = sut.StoreRequest(sessionId, "/test", HttpMethod.Get, "test".ToAsciiBytes());

            Expression<Func<Request, bool>> expr = r => r.Content.Contains("test");

            var responseId = sut.StoreResponse(requestId, "test", expr.ToString(), "test".ToAsciiBytes());

            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());

            var response = dynamo.GetItem<Responses>(responseId.ToString());

            response.ImposterName
                    .Should().Be("test");
            response.RequestId
                    .Should().Be(requestId);
            response.ResponsePayloadBase64
                    .Should().Be("test".ToAsciiBytes().ToBase64String());
            response.MatchedCondition
                    .Should().Be(expr.ToString());
        }

        private static AwsDynamoDbDataStore CreateSut()
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
