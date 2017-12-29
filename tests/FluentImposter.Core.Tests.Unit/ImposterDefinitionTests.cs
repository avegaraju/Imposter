using System;
using System.Linq;
using System.Linq.Expressions;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class ImposterDefinitionTests
    {
        [Fact]
        public void ImposterDefinition_ImposterOfTypeRestCanBeCreated()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.IsOfType(ImposterType.REST)
                                             .StubsResource("/test")
                                             .Build();

            imposter.Type
                    .Should().Be(ImposterType.REST);
        }

        [Fact]
        public void ImposterDefinition_ImposterCanStubAResource()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.IsOfType(ImposterType.REST)
                                             .StubsResource("/test")
                                             .Build();

            imposter.Resource
                    .Should().Be("/test");
        }

        [Fact]
        public void HasAnImposter_ImposterConditionsAreCorrectlyAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.IsOfType(ImposterType.REST)
                                             .StubsResource("/test")
                                             .When(r => r.Body.Content.Contains(""))
                                             .Then(new DefaultResponseCreator().CreateResponse())
                                             .Build();

            Expression<Func<Request, bool>> expectedCondition = r => r.Body.Content.Contains("");

            imposter.Rules.First().Condition
                    .Should().BeEquivalentTo(expectedCondition);
        }

        [Fact]
        public void ImposterDefinition_MultipleRules_CorrectlyGetsAddedToTheImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.IsOfType(ImposterType.REST)
                                             .StubsResource("/test")
                                             .When(r => r.Body.Content.Contains(""))
                                             .Then(new DefaultResponseCreator().CreateResponse())
                                             .When(r => r.Body.Content.StartsWith("test"))
                                             .Then(new TestResponseCreator().CreateResponse())
                                             .Build();

            var firstRule = new Rule();
            firstRule.SetCondition(r => r.Body.Content.Contains(""));
            firstRule.SetAction(new DefaultResponseCreator().CreateResponse());

            var secondRule = new Rule();
            secondRule.SetCondition(r => r.Body.Content.StartsWith("test"));
            secondRule.SetAction(new TestResponseCreator().CreateResponse());

            imposter.Rules
                    .Should().BeEquivalentTo(new[]
                                             {
                                                 firstRule,
                                                 secondRule
                                             });
        }

        [Fact]
        public void HasAnImposter_ImposterResponseIsCorrectlyAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.IsOfType(ImposterType.REST)
                                             .StubsResource("/test")
                                             .When(r => r.Body.Content.Contains(""))
                                             .Then(new DefaultResponseCreator().CreateResponse())
                                             .Build();

            var expectedAction = new DefaultResponseCreator().CreateResponse();

            imposter.Rules.First().Action
                    .Should().BeEquivalentTo(expectedAction);
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
