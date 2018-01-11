using System.Net.Http;

using FluentAssertions;

using FluentImposter.AspnetCore.Tests.Integration.Fakes;

using Xunit;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    public class ApplicationBuilderExtensionTests
    {
        [Fact]
        public async void Middleware_ImposterReceivesRequestWithMatchingContent_ReturnsPreDefinedResponse()
        {
            using (var testServer = new TestServerBuilder()
                    .UsingImpostersMiddleware(new FakeImposterWithRequestContent().Build())
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
                    .UsingImpostersMiddleware(new FakeImposterWithRequestHeader().Build())
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
                    .UsingImpostersMiddleware(new FakeImposterWithRequestHeaderAndContent().Build())
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
}
