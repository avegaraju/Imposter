
using System.Net.Http;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class RulesEvaluatorTests
    {
        private const int INTERNAL_SERVER_ERROR = 500;
        private const int STATUS_CODE_OK = 200;

        [Fact]
        public void Evaluate_WhenRequestContentDoesNotMatchCondition_ReturnsInternalServerError()
        {
            string responseContent = "None of evaluators could create a response.";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.Content.Contains("none of the imposter conditions will be able to " +
                                                  "match this text"))
                    .Then(new DummyResponseCreator(responseContent, INTERNAL_SERVER_ERROR))
                    .Build();

            var request = new RequestBuilder()
                    .WithRequestContent("test content")
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(INTERNAL_SERVER_ERROR);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_WhenRequestHeaderDoesNotExistsAndConditionIsDefinedForRequestHeader_ReturnsInternalServerError()
        {
            string responseContent = "None of evaluators could create a response.";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept", new []
                                                                              {
                                                                                  "text/plain",
                                                                                  "test/xml"
                                                                              }))
                    .Then(new DummyResponseCreator(responseContent, INTERNAL_SERVER_ERROR))
                    .Build();

            var request = new RequestBuilder()
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(INTERNAL_SERVER_ERROR);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_WhenRequestContentDoesNotExistsAndConditionIsDefinedForRequestContent_ReturnsInternalServerError()
        {
            string responseContent = "None of evaluators could create a response.";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.Content.Contains("none of the imposter conditions will be able to " +
                                                  "match this text"))
                    .Then(new DummyResponseCreator(responseContent, INTERNAL_SERVER_ERROR))
                    .Build();

            var request = new RequestBuilder()
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(INTERNAL_SERVER_ERROR);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_WhenRequestContentMatchesTheCondition_ReturnsPreDefinedResponse()
        {
            string requestContent = "dummy request";
            string responseContent = "dummy response.";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.Content.Contains(requestContent))
                    .Then(new DummyResponseCreator(responseContent, STATUS_CODE_OK))
                    .Build();

            var request = new RequestBuilder()
                    .WithRequestContent(requestContent)
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(STATUS_CODE_OK);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_WhenRequestHeaderMatchesTheCondition_ReturnsPreDefinedResponse()
        {
            string headerKey = "Accept";
            var headerValues = new string[]
                              {
                                  "text/xml",
                                  "text/plain"
                              };

            string responseContent = "dummy response.";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.RequestHeader.ContainsKeyAndValues(headerKey, headerValues))
                    .Then(new DummyResponseCreator(responseContent, STATUS_CODE_OK))
                    .Build();

            var request = new RequestBuilder()
                    .WithRequestHeader(headerKey, headerValues)
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(STATUS_CODE_OK);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_WhenRequestHeaderMatchesButRequestContentDoesNotMatchTheCondition_ReturnsPreDefinedResponse()
        {
            string headerKey = "Accept";
            var headerValues = new string[]
                              {
                                  "text/xml",
                                  "text/plain"
                              };

            string requestContent = "this content will not match the condition";
            string responseContent = "dummy response.";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.RequestHeader.ContainsKeyAndValues(headerKey, headerValues))
                    .Then(new DummyResponseCreator(responseContent, STATUS_CODE_OK))
                    .When(r=> r.Content.Contains(requestContent))
                    .Then(new DummyResponseCreator(responseContent, STATUS_CODE_OK))
                    .Build();

            var request = new RequestBuilder()
                    .WithRequestHeader(headerKey, headerValues)
                    .WithRequestContent("dummy request")
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(STATUS_CODE_OK);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_PredefinedResponseCorrespondingToTheMatchedHeaderConditionIsReturned()
        {
            string headerKey = "Accept";
            var headerValues = new string[]
                              {
                                  "text/xml",
                                  "text/plain"
                              };

            string requestContent = "this content will not match the condition";
            string responseContent = "dummy response for matched header";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.RequestHeader.ContainsKeyAndValues(headerKey, headerValues))
                    .Then(new DummyResponseForMatchedHeader(responseContent, STATUS_CODE_OK))
                    .When(r => r.Content.Contains(requestContent))
                    .Then(new DummyResponseCreator(responseContent, STATUS_CODE_OK))
                    .Build();

            var request = new RequestBuilder()
                    .WithRequestHeader(headerKey, headerValues)
                    .WithRequestContent("dummy request")
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(STATUS_CODE_OK);
            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void Evaluate_PredefinedResponseCorrespondingToTheMatchedContentConditionIsReturned()
        {
            string headerKey = "this_key_doesnt_not_match";
            var headerValues = new string[]
                              {
                                  "text1/xml1",
                                  "text1/plain1"
                              };

            string requestContent = "dummy request";
            string responseContent = "dummy response for matched header";

            var imposter = new ImposterDefinition("test")
                    .StubsResource("/test", HttpMethod.Post)
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept", new[]{"test"}))
                    .Then(new DummyResponseForMatchedHeader(responseContent, STATUS_CODE_OK))
                    .When(r => r.Content.Contains(requestContent))
                    .Then(new DummyResponseCreator(responseContent, STATUS_CODE_OK))
                    .Build();

            var request = new RequestBuilder()
                    .WithRequestHeader(headerKey, headerValues)
                    .WithRequestContent(requestContent)
                    .Build();

            var response = RulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(STATUS_CODE_OK);
            response.Content.Should().Be(responseContent);
        }

        internal class DummyResponseCreator: IResponseCreator
        {
            private readonly string _content;
            private readonly int _statusCode;

            public DummyResponseCreator(string content, int statusCode)
            {
                _content = content;
                _statusCode = statusCode;
            }

            public Response CreateResponse()
            {
                return new Response()
                       {
                           Content = _content,
                           StatusCode = _statusCode
                       };
            }
        }

        internal class DummyResponseForMatchedHeader: IResponseCreator
        {
            private readonly string _content;
            private readonly int _statusCode;

            public DummyResponseForMatchedHeader(string content, int statusCode)
            {
                _content = content;
                _statusCode = statusCode;
            }

            public Response CreateResponse()
            {
                return new Response()
                       {
                           Content = _content,
                           StatusCode = _statusCode
                       };
            }
        }
    }
}


