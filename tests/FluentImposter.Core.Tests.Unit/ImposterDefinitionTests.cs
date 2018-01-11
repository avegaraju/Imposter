using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class ImposterDefinitionTests
    {
        [Fact]
        public void ImposterDefinition_ImposterCanStubAResource()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.StubsResource("/test", HttpMethod.Post)
                    .Build();

            imposter.Resource
                    .Should().Be("/test");
            imposter.Behavior
                    .Should().Be(ImposterBehavior.Stub);
        }

        [Fact]
        public void ImposterDefinition_ImposterCanMockAResource()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.MocksResource("/test", HttpMethod.Post)
                    .Build();

            imposter.Resource
                    .Should().Be("/test");
            imposter.Behavior
                    .Should().Be(ImposterBehavior.Mock);
        }

        [Fact]
        public void ImposterDefinition_AllowsToDefineHttpMethod()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.MocksResource("/test", HttpMethod.Post)
                                             .Build();

            imposter.Method
                    .Should().Be(HttpMethod.Post);
        }

        [Fact]
        public void ImposterDefinition_ImposterConditionsAreCorrectlyAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.StubsResource("/test", HttpMethod.Post)
                                             .When(r => r.Content.Contains(""))
                                             .Then(new DefaultResponseCreator())
                                             .Build();

            Expression<Func<Request, bool>> expectedCondition = r => r.Content.Contains("");

            imposter.Rules.First().Condition
                    .Should().BeEquivalentTo(expectedCondition);
        }

        [Fact]
        public void ImposterDefinition_MultipleRules_CorrectlyGetsAddedToTheImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.StubsResource("/test", HttpMethod.Post)
                                             .When(r => r.Content.Contains(""))
                                             .Then(new DefaultResponseCreator())
                                             .When(r => r.Content.StartsWith("test"))
                                             .Then(new TestResponseCreator())
                                             .Build();

            var firstRule = new Rule();
            firstRule.SetCondition(r => r.Content.Contains(""));
            firstRule.SetAction(new DefaultResponseCreator());

            var secondRule = new Rule();
            secondRule.SetCondition(r => r.Content.StartsWith("test"));
            secondRule.SetAction(new TestResponseCreator());

            imposter.Rules
                    .Should().BeEquivalentTo(new[]
                                             {
                                                 firstRule,
                                                 secondRule
                                             });
        }

        [Fact]
        public void ImposterDefinition_WhenConditionDefinedOnRequestHeader_ConditionGetsAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.StubsResource("/test", HttpMethod.Post)
                                             .When(r => r.RequestHeader.Contains("Accept"))
                                             .Then(new DefaultResponseCreator())
                                             .Build();

            Expression<Func<Request, bool>> requestCondition = r => r.RequestHeader.Contains("Accept");

            imposter.Rules.First().Condition
                    .Should().BeEquivalentTo(requestCondition);
        }

        [Fact]
        public void ImposterDefinition_ImposterResponseIsCorrectlyAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.StubsResource("/test", HttpMethod.Post)
                                             .When(r => r.Content.Contains(""))
                                             .Then(new DefaultResponseCreator())
                                             .Build();

            var expectedAction = new DefaultResponseCreator();

            imposter.Rules.First().ResponseCreatorAction.CreateResponse()
                    .Should().BeEquivalentTo(expectedAction.CreateResponse());
        }

        private static ImposterDefinition CreateSut()
        {
            return new ImposterDefinition("test");
        }

        private class DefaultResponseCreator : IResponseCreator
        {
            public Response CreateResponse()
            {
                return new Response();
            }
        }

        private class TestResponseCreator : IResponseCreator
        {
            public Response CreateResponse()
            {
                return new Response();
            }
        }
    }
}
