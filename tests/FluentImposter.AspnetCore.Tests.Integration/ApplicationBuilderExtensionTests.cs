using System.Net.Http;

using FluentAssertions;

using Xunit;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    public class ApplicationBuilderExtensionTests
    {
        [Fact]
        public async void Middleware_ImposterReceivesRequest_ReturnsExpectedResponse()
        {
            using (var testServer = new TestServerBuilder()
                    .UsingImpostersMiddleware()
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
    }
}
