using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;

using FluentAssertions;

using FluentImposter.AspnetCore.Tests.Integration.Fakes;
using FluentImposter.AspnetCore.Tests.Integration.Spies;
using FluentImposter.Core.Entities;

using Newtonsoft.Json;

using Xunit;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    public class ApplicationBuilderExtensionTests
    {
        [Fact]
        public async void Middleware_ImposterReceivesRequestWithMatchingContent_ReturnsPreDefinedResponse()
        {
            using (var testServer = new TestServerBuilder()
                    .UsingImpostersMiddleware(new FakeImposterWithRequestContent(HttpMethod.Get).Build())
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("test")
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
                                       .CreateRequest("test")
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
                                       .CreateRequest("test")
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
        public async void Middleware_ImposterReceivesMockSessionCreationRequest_WhenTheMethodIsGet_ReturnsNotFound()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent(HttpMethod.Get).Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("mocks/session")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .GetAsync();

                response.StatusCode
                        .Should().Be(HttpStatusCode.NotFound);
                spyDataStore.NewSessionId
                            .Should().Be(Guid.Empty);
            }
        }

        [Fact]
        public async void
                Middleware_ImposterReceivesMockSessionCreationRequest_WhenTheMethodIsPost_ReturnsCreatedWithValidSession()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/mocks/session")
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

        [Fact]
        public async void
                Middleware_ImposterReceivesMockSessionCreationRequest_AndTheExistingSessionIsStillActive_EndsThePreviousSession()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                var firstSessionId = spyDataStore.NewSessionId;

                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                spyDataStore.EndedSessionId.Should().Be(firstSessionId);
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_RequestWillHaveAnActiveSessionId()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var sessionResponse = await testServer
                                              .CreateRequest("/mocks/session")
                                              .And(message =>
                                                   {
                                                       message.Content =
                                                               new StringContent("dummy request");
                                                   })
                                              .PostAsync();

                var activeSessionId = sessionResponse.Content.ReadAsStringAsync().Result;

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                spyDataStore.SessionIdReceivedWithRequest.Should().Be(activeSessionId);
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectResourceStringIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                spyDataStore.RequestedResource.Should().Be("test");
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectHtpMethodIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var sessionResponse = await testServer
                                              .CreateRequest("/mocks/session")
                                              .And(message =>
                                                   {
                                                       message.Content =
                                                               new StringContent("dummy request");
                                                   })
                                              .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                spyDataStore.HttpMethod.Should().Be(HttpMethod.Post.ToString());
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectRequestPayloadIsReceivedByTheDataStore()
        {
            var requestContent = "dummy request";
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent(requestContent);
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                Encoding.ASCII.GetString(spyDataStore.RequestPayload)
                        .Should().Be(requestContent);
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_EachResponseWillHaveARequestId()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                spyDataStore.RequestIdReceivedWhileStoringResponse
                            .Should().Be(spyDataStore.NewRequestId);
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectResponseConditionIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                Expression<Func<Request, bool>> condition = r => r.Content.Contains("dummy");

                spyDataStore.MatchedCondition
                            .Should().Be(condition.ToString());
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectImposterNameIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                spyDataStore.ImposterName
                            .Should().Be("test");
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectResponsePayloadIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                Encoding.ASCII
                        .GetString(spyDataStore.ResponsePayload).Should().Be("dummy response");
            }
        }

        [Fact]
        public async void Middleware_WhenDataStoreIsNotConfigured_ReturnsResponseWithoutAnyErrors()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("test")
                                       .And(message =>
                                            {
                                                message.Content = new StringContent("dummy request");
                                            })
                                       .PostAsync();

                response.Content
                        .ReadAsStringAsync().Result.Should().Be("dummy response");
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_MockVerificationClosesSessionImplicitly()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/mocks/session")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .PostAsync();

                var sessionId = response.Content.ReadAsStringAsync().Result;

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest($"mocks/{sessionId}/verify")
                        .And(msg =>
                             {
                                 msg.Content = new StringContent("{resource:'test'");
                             })
                        .GetAsync();

                spyDataStore.EndedSessionId
                            .Should().Be(sessionId);
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_VerifiesTheCallIsMade()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/mocks/session")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .PostAsync();

                var sessionId = response.Content.ReadAsStringAsync().Result;

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                var verificationResponse = await testServer
                                                   .CreateRequest($"mocks/{sessionId}/verify")
                                                   .And(msg =>
                                                        {
                                                            msg.Content = new StringContent("{resource:'test'}");
                                                        })
                                                   .GetAsync();

                var verificationResponseObject = JsonConvert
                        .DeserializeObject<List<VerificationResponse>>
                        (verificationResponse.Content.ReadAsStringAsync().Result);

                verificationResponseObject
                        .Should().HaveCount(1);

                verificationResponseObject [0]
                        .Resource.Should().Be("test");
            }
        }

        [Fact]
        public async void Middleware_WithInvalidSessionIdInTheUri_ReturnsBadRequest()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("/mocks/session")
                        .And(message =>
                             {
                                 message.Content =
                                         new StringContent("dummy request");
                             })
                        .PostAsync();

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                var verificationResponse = await testServer
                                                   .CreateRequest("mocks/abnnndbbd/verify")
                                                   .And(msg =>
                                                        {
                                                            msg.Content = new StringContent("{resource:'test'}");
                                                        })
                                                   .GetAsync();

                verificationResponse.StatusCode
                                    .Should().Be(HttpStatusCode.BadRequest);

                verificationResponse.Content
                                    .ReadAsStringAsync().Result.Should().Contain("abnnndbbd");
            }
        }

        [Fact]
        public async void Middleware_WithInvalidVerificationRequest_ReturnsBadRequest()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("/mocks/session")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .PostAsync();

                var sessionId = response.Content.ReadAsStringAsync().Result;

                await testServer
                        .CreateRequest("test")
                        .And(message =>
                             {
                                 message.Content = new StringContent("dummy request");
                             })
                        .PostAsync();

                var verificationResponse = await testServer
                                                   .CreateRequest($"mocks/{sessionId}/verify")
                                                   .And(msg =>
                                                        {
                                                            msg.Content = new StringContent("nnnn:bbbb");
                                                        })
                                                   .GetAsync();

                verificationResponse.StatusCode
                                    .Should().Be(HttpStatusCode.BadRequest);

                verificationResponse.Content.ReadAsStringAsync().Result
                                    .Should().Contain("not a valid JSON");
            }
        }

        [Fact]
        public async void Middleware_ImposterCanStubbingAResourceWithAPatternInUri()
        {
            SpyDataStore spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterStubbingAResourceWithAPatternInUri(HttpMethod
                                                                                                                     .Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                var response = await testServer
                                       .CreateRequest("products/1?requestid=12344")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("dummy request");
                                            })
                                       .PostAsync();

                response.Content.ReadAsStringAsync().Result
                        .Should().Be("dummy response");
            }
        }
    }
}
