using System;
using System.Collections.Generic;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Microsoft.Extensions.Primitives;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class RulesEvaluatorTests
    {
        private const int INTERNAL_SERVER_ERROR_CODE = 500;

        [Fact]
        public void Ctor_WithNullEvaluators_ReturnsArgumentNullException()
        {
            Action exceptionThrowingAction = () => new RulesEvaluator(null);

            exceptionThrowingAction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_WhenOneOfTheEvaluatorsIsNull_ReturnsArgumentNullException()
        {
            Action exceptionThrowingAction = () => new RulesEvaluator(new IEvaluator[]{null,new DummyEvaluator()});

            exceptionThrowingAction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Evaluate_WhenAnEvaluatorReturnsNullResponse_ReturnsInternalServerErrorResponse()
        {
            var rulesEvaluator = new RulesEvaluator(new IEvaluator[]
                                                    {
                                                        new DummyEvaluator(),
                                                        new NullResponseReturningEvaluator()
                                                    });

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

            var response = rulesEvaluator.Evaluate(imposter, request);

            response.StatusCode.Should().Be(INTERNAL_SERVER_ERROR_CODE);
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

    internal class DummyEvaluator: IEvaluator
    {
        public EvaluationResult Evaluate(Imposter imposter, Request request)
        {
            return new EvaluationResult(RuleEvaluationOutcome.FoundAMatch,
                                        new Response());
        }
    }

    internal class NullResponseReturningEvaluator: IEvaluator
    {
        EvaluationResult IEvaluator.Evaluate(Imposter imposter, Request request)
        {
            return new EvaluationResult(RuleEvaluationOutcome.NoMatchesFound, null);
        }
    }
}
