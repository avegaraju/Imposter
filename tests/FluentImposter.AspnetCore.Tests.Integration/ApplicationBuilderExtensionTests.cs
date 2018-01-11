using System;
using System.Net;
using System.Net.Http;

using FluentAssertions;

using FluentImposter.AspnetCore.Tests.Integration.Fakes;
using FluentImposter.AspnetCore.Tests.Integration.Spies;

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
                                       .CreateRequest("/test/sklskdlkl")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy");
                                            })
                                       .GetAsync();

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

        [Fact]
        public async void Middleware_ImposterReceivesMockSessionCreationRequest_WhenTheMethodIsGet_ReturnsBadRequest()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent().Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/mock/createsession")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .GetAsync();

                response.StatusCode
                        .Should().Be(HttpStatusCode.BadRequest);
                spyDataStore.NewSessionId
                            .Should().Be(Guid.Empty);
            }
        }

        [Fact]
        public async void Middleware_ImposterReceivesMockSessionCreationRequest_WhenTheMethodIsPost_ReturnsCreatedWithValidSession()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent().Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/mock/createsession")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .PostAsync();

                response.StatusCode
                        .Should().Be(HttpStatusCode.Created);

                spyDataStore.NewSessionId
                    .Should().Be(response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
