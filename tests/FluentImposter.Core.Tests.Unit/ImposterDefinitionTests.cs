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

            var imposter = imposterDefinition
                .ForRest()
                .DeclareResource("/test", HttpMethod.Post)
                .Build();

            imposter.Method
                    .Should().Be(HttpMethod.Post);
        }

        [Fact]
        public void ImposterDefinition_ImposterConditionsAreCorrectlyAddedToImposter()
        {
            ImposterDefinition imposterDefinition = CreateSut();

            var imposter = imposterDefinition
                .ForRest()
                .DeclareResource("/test", HttpMethod.Post)
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

            var imposter = imposterDefinition.ForRest()
                .DeclareResource("/test", HttpMethod.Post)
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

            var imposter = imposterDefinition.ForRest()
                .DeclareResource("/test", HttpMethod.Post)
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

            var imposter = imposterDefinition.ForRest()
                .DeclareResource("/test", HttpMethod.Post)
                .When(r => r.Content.Contains(""))
                .Then(new DefaultResponseCreator())
                .Build();

            var expectedAction = new DefaultResponseCreator();

            imposter.Rules.First().ResponseCreatorAction.CreateResponse()
                    .Should().BeEquivalentTo(expectedAction.CreateResponse());
        }

        [Fact]
        public void ImposterDefinition_AllowsToDefineRestImposter()
        {
            var imposterDefinition = CreateSut();

            var restResource= imposterDefinition.ForRest();
            
            restResource.Should().BeOfType<RestResource>();
        }

        [Fact]
        public void ImposterDefinition_AllowsToDefineSmtpImposter()
        {
            var imposterDefinition = CreateSut();

            var smtpServer = imposterDefinition.ForSmtp();

            smtpServer.Should().BeOfType<SmtpServer>();
        }

        [Fact]
        public void ImposterDefinition_AllowsToCreateASmtpServerAtAUri()
        {
            var expected = new Uri("http://localhost");

            var imposterDefinition = CreateSut();

            var smtpServer = imposterDefinition
                .ForSmtp()
                .CreateServer(expected);

            smtpServer.SmtpServerUri.Should().Be(expected);
        }

        [Fact]
        public void ImposterDefinition_AllowsToDefineAPortForSmtpServer()
        {
            var uri = new Uri("http://localhost");
            uint expectedPort = 225;

            var imposterDefinition = CreateSut();

            var smtpServer = imposterDefinition
                .ForSmtp()
                .CreateServer(uri)
                .AtPort(expectedPort);

            smtpServer.Port.Should().Be(expectedPort);
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
