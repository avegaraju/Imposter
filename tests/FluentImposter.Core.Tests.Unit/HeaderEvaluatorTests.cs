using System.Collections.Generic;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Microsoft.Extensions.Primitives;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class HeaderEvaluatorTests
    {
        [Fact]
        public void Evaluate_WhenRequestHeaderKeyDoesNotMatchCondition_ReturnsInternalServerErrorWithAppropriateMessage()
        {
            string responseContent = "if match found, this text will be returned.";
            var imposter = new ImposterDefinition("test")
                    .IsOfType(ImposterType.REST)
                    .StubsResource("/test")
                    .When(r => r.RequestHeader.Contains("this_key_will_not_match"))
                    .Then(new DummyResponseCreator(responseContent))
                    .Build();

            List<KeyValuePair<string, StringValues>> keyValuePairs
                    = new List<KeyValuePair<string, StringValues>>
                      {
                          new KeyValuePair<string, StringValues>("Accept",
                                                                 new StringValues(new[]
                                                                                  {
                                                                                      "text/plain",
                                                                                      "text/xml"
                                                                                  }))
                      };

            var request = BuildRequestWithHeaders(keyValuePairs);

            var evaluationResult = new HeaderEvaluator().Evaluate(imposter, request);

            evaluationResult.Response.Content.Should().Be("None of the imposter conditions matched.");
            evaluationResult.Response.StatusCode.Should().Be(500);
        }

        [Fact]
        public void Evaluate_WhenRequestHeaderKeyMatchesButValuesDoesNotMatchCondition_ReturnsInternalServerErrorWithAppropriateMessage()
        {
            string responseContent = "if match found, this text will be returned.";
            var imposter = new ImposterDefinition("test")
                    .IsOfType(ImposterType.REST)
                    .StubsResource("/test")
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept", new []{ "this_value_will_not_match" }))
                    .Then(new DummyResponseCreator(responseContent))
                    .Build();

            List<KeyValuePair<string, StringValues>> keyValuePairs
                    = new List<KeyValuePair<string, StringValues>>
                      {
                          new KeyValuePair<string, StringValues>("Accept",
                                                                 new StringValues(new[]
                                                                                  {
                                                                                      "text/plain",
                                                                                      "text/xml"
                                                                                  }))
                      };

            var request = BuildRequestWithHeaders(keyValuePairs);

            var evaluationResult = new HeaderEvaluator().Evaluate(imposter, request);

            evaluationResult.Response.Content.Should().Be("None of the imposter conditions matched.");
            evaluationResult.Response.StatusCode.Should().Be(500);
        }

        [Fact]
        public void Evaluate_WhenRequestHeaderKeyMatchesTheCondition_ReturnsAppropriateResponse()
        {
            string responseContent = "if match found, this text will be returned.";
            var imposter = new ImposterDefinition("test")
                    .IsOfType(ImposterType.REST)
                    .StubsResource("/test")
                    .When(r => r.RequestHeader.Contains("Accept"))
                    .Then(new DummyResponseCreator(responseContent))
                    .Build();

            List<KeyValuePair<string, StringValues>> keyValuePairs
                    = new List<KeyValuePair<string, StringValues>>
                      {
                          new KeyValuePair<string, StringValues>("Accept",
                                                                 new StringValues(new[]
                                                                                  {
                                                                                      "text/plain",
                                                                                      "text/xml"
                                                                                  }))
                      };

            var request = BuildRequestWithHeaders(keyValuePairs);

            var evaluationResult = new HeaderEvaluator().Evaluate(imposter, request);

            evaluationResult.Response.Content.Should().Be(responseContent);
            evaluationResult.Response.StatusCode.Should().Be(200);
        }

        private Request BuildRequestWithHeaders(List<KeyValuePair<string, StringValues>> keyValuePairs)
        {
            var requestHeader = new RequestHeader();

            foreach (KeyValuePair<string, StringValues> keyValuePair in keyValuePairs)
            {
                requestHeader.Add(keyValuePair.Key, keyValuePair.Value.ToArray());
            }

            return new Request()
                   {
                       RequestHeader = requestHeader
                   };
        }
    }
}
