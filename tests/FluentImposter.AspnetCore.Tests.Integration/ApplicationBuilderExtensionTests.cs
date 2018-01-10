using System.Net.Http;

using FluentAssertions;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    public class ApplicationBuilderExtensionTests
    {
        [Fact]
        public async void Middleware_ImposterReceivesRequestWithMatchingContent_ReturnsPreDefinedResponse()
        {
            using (var testServer = new TestServerBuilder()
                    .UsingImpostersMiddleware(new DummyImposterWithRequestContent().Build())
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/test")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy");
                                            })
                                       .PostAsync();

                var content = response.Content.ReadAsStringAsync().Result;

                content.Should().Be("dummy response");
            }
        }

        [Fact]
        public async void Middleware_ImposterReceivesRequestWithMatchingHeader_ReturnsPreDefinedResponse()
        {
            using (var testServer = new TestServerBuilder()
                    .UsingImpostersMiddleware(new DummyImposterWithRequestHeader().Build())
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/test")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy");
                                            })
                                       .AddHeader("Accept", "text/plain")
                                       .AddHeader("Accept", "text/xml")
                                       .PostAsync();

                var content = response.Content.ReadAsStringAsync().Result;

                content.Should().Be("dummy response");
            }
        }

        [Fact]
        public async void Middleware_ImposterReceivesRequestWithoutAnyMatchingConditions_ReturnsInternalServerError()
        {
            using (var testServer = new TestServerBuilder()
                    .UsingImpostersMiddleware(new DummyImposterWithRequestHeaderAndContent().Build())
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/test")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("This content will not match");
                                            })
                                       .AddHeader("this_key_wont_match", "this_too_wont_match")
                                       .PostAsync();

                response.Content.ReadAsStringAsync().Result.Should().Be("None of evaluators could create a response.");
            }
        }
    }

    internal class DummyImposterWithRequestContent : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("/test")
                    .When(r => r.Content.Contains("dummy"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }

    internal class DummyImposterWithRequestHeader : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("/test")
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept",
                                                                    new[]
                                                                    {
                                                                        "text/plain",
                                                                        "text/xml"
                                                                    }))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }

    internal class DummyImposterWithRequestHeaderAndContent : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("/test")
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept",
                                                                    new[]
                                                                    {
                                                                        "text/plain",
                                                                        "text/xml"
                                                                    }))
                    .Then(new DummyResponseCreator())
                    .When(r => r.Content.Contains("dummy request"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
