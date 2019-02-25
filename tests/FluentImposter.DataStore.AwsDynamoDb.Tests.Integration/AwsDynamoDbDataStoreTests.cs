using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using FluentAssertions;

using FluentImposter.Core.Entities;
using FluentImposter.Core.Exceptions;
using FluentImposter.Core.Models;

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
            CreateSut();

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
        public void StoreRequest_StoresTheRequestAlongWithInvocationCount()
        {
            var sut = CreateSut();

            sut.PurgeData<Requests>();

            sut.StoreRequest("/test",
                             HttpMethod.Post,
                             "test".ToAsciiBytes());

            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());
            var queryResult = dynamo.GetAll<Requests>().First(r => r.Resource.Equals("/test")
                                                 && r.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)
                                                 && r.RequestPayloadBase64
                                                     .Equals(Convert.ToBase64String("test".ToAsciiBytes())));
            
            queryResult.HttpMethod
                   .Should().Be(HttpMethod.Post.ToString());
            queryResult.Resource
                   .Should().Be("/test");
            queryResult.RequestPayloadBase64
                   .Should().Be("test".ToAsciiBytes().ToBase64String());
            queryResult.InvocationCount.Should().Be(1);
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void StoreResponse_WithIncorrectRequestId_ThrowsException()
        {
            var sut = CreateSut();

            Action exceptionThrowingAction
                    = () => sut.StoreResponse(Guid.NewGuid(), "test", "condition expression", null);

            exceptionThrowingAction
                    .Should().Throw<RequestDoesNotExistException>();
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void StoreResponse_WithValidRequestId_StoresResponse()
        {
            var sut = CreateSut();

            sut.PurgeData<Requests>();
            sut.PurgeData<Responses>();

            var requestId = sut.StoreRequest("/test", HttpMethod.Get, "test".ToAsciiBytes());

            Expression<Func<Request, bool>> expr = r => r.Content.Contains("test");

            sut.StoreResponse(requestId, "test", expr.ToString(), "test".ToAsciiBytes());

            var dynamo = new PocoDynamo(CreateAmazonDynamoDbClientWithLocalDbInstance());

            var response = dynamo.Scan<Responses>(new ScanRequest("Responses"))
                                 .First(r => r.RequestId == requestId);

            response.ImposterName
                    .Should().Be("test");
            response.RequestId
                    .Should().Be(requestId);
            response.ResponsePayloadBase64
                    .Should().Be("test".ToAsciiBytes().ToBase64String());
            response.MatchedCondition
                    .Should().Be(expr.ToString());
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void GetVerificationResponse_WhenResourceIsFound_ReturnsVerificationResponses()
        {
            var sut = CreateSut();

            sut.PurgeData<Requests>();
            sut.PurgeData<Responses>();

            var requestId = sut.StoreRequest("/testResource", HttpMethod.Get, "test".ToAsciiBytes());

            Expression<Func<Request, bool>> expr = r => r.Content.Contains("test");

            sut.StoreResponse(requestId, "test", expr.ToString(), "test".ToAsciiBytes());

            var verificationReponses = sut.GetVerificationResponse("/testResource", HttpMethod.Get, "test".ToAsciiBytes());

            var expectedObject =
                    new VerificationResponse()
                    {
                        Resource = "/testResource",
                        InvocationCount = 1,
                        RequestPayload = "test"
                    };

            verificationReponses
                    .Should().BeEquivalentTo(expectedObject);
        }

        [Fact]
        [Trait("Category", "DynamoDb")]
        public void GetVerificationResponse_WhenResourceIsFoundMultipleTimes_ReturnsVerificationResponsesWithCorrectInvocationCount()
        {
            var sut = CreateSut();

            sut.PurgeData<Requests>();
            sut.PurgeData<Responses>();

            var requestId1 = sut.StoreRequest("/testResource", HttpMethod.Get, "test".ToAsciiBytes());
            var requestId2 = sut.StoreRequest("/testResource", HttpMethod.Get, "test".ToAsciiBytes());

            Expression<Func<Request, bool>> expr = r => r.Content.Contains("test");

            sut.StoreResponse(requestId1, "test", expr.ToString(), "test".ToAsciiBytes());
            sut.StoreResponse(requestId2, "test", expr.ToString(), "test".ToAsciiBytes());

            var verificationReponses = sut.GetVerificationResponse("/testResource", HttpMethod.Get, "test".ToAsciiBytes());

            var expectedObject =
                    new VerificationResponse()
                    {
                        Resource = "/testResource",
                        InvocationCount = 2,
                        RequestPayload = "test"
                    };

            verificationReponses
                    .Should().BeEquivalentTo(expectedObject);
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
