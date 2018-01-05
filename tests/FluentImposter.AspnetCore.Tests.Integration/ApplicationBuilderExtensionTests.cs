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
        public async void Middleware_ImposterReceivesRequestWithMatchingContent_ReturnsExpectedResponse()
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
        public async void Middleware_ImposterReceivesRequestWithMatchingHeader_ReturnsExpectedResponse()
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
    }

    internal class DummyImposterWithRequestContent : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .IsOfType(ImposterType.REST)
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
                    .IsOfType(ImposterType.REST)
                    .StubsResource("/test")
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept", new[]
                                                                              {
                                                                                "text/plain",
                                                                                "text/xml"
                                                                              }))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
