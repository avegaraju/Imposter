using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Mail;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class ImposterDefinitionTests
    {
        [Fact]
        public void ImposterDefinition_AllowsToDefineHttpMethod()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.DeclareResource("/test", HttpMethod.Post)
                                             .Build();

            imposter.Method
                    .Should().Be(HttpMethod.Post);
        }

        [Fact]
        public void ImposterDefinition_ImposterConditionsAreCorrectlyAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition.DeclareResource("/test", HttpMethod.Post)
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

            var imposter = imposterDefinition.DeclareResource("/test", HttpMethod.Post)
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

            var imposter = imposterDefinition.DeclareResource("/test", HttpMethod.Post)
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

            var imposter = imposterDefinition.DeclareResource("/test", HttpMethod.Post)
                                             .When(r => r.Content.Contains(""))
                                             .Then(new DefaultResponseCreator())
                                             .Build();

            var expectedAction = new DefaultResponseCreator();

            imposter.Rules.First().ResponseCreatorAction.CreateResponse()
                    .Should().BeEquivalentTo(expectedAction.CreateResponse());
        }

        [Theory]
        [InlineData(ImposterOfType.Rest)]
        [InlineData(ImposterOfType.Smtp)]
        public void ImposterDefinition_AllowsToChooseImposterType(ImposterOfType type)
        {
            var imposterDefinition = CreateSut();

            var imposter = imposterDefinition.OfType(type);

            using var scope = new AssertionScope();

            switch (type)
            {
                case ImposterOfType.Rest:
                    imposter.Should().BeOfType<RestImposter>();
                    break;
                case ImposterOfType.Smtp:
                    imposter.Should().BeOfType<SmtpImposter>();
                    break;
            }
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
