using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ApplicationBuilderExtensionMockTests
    {
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
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                spyDataStore.Requests.Should()
                            .Contain(r => r.Resource == "test");
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
                await testServer
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                spyDataStore.Requests.Should().Contain(r=>r.HttpMethod == HttpMethod.Post.ToString());
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
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent(requestContent);
                        })
                        .PostAsync();

                var expectedBase64RequestPayload = Convert.ToBase64String(Encoding.ASCII.GetBytes(requestContent));

                spyDataStore.Requests.Should()
                            .Contain(r => r.RequestPayloadBase64.Equals(expectedBase64RequestPayload));
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectMatchedConditionIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                Expression<Func<Request, bool>> condition = r => r.Content.Contains("dummy");

                spyDataStore.Responses
                            .Should().Contain(r => r.MatchedCondition.Equals(condition.ToString()));
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectResourceIsReceivedByTheDataStore()
        {
            var resource = "test";

            var imposter = new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                    .Build();

            var spyDataStore = new SpyDataStore();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(imposter,
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest(resource)
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                spyDataStore.Responses
                            .Should().Contain(r => r.ImposterName.Equals(imposter.Name));
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_CorrectResponsePayloadIsReceivedByTheDataStore()
        {
            var spyDataStore = new SpyDataStore();

            var imposter = new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                    .Build();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(imposter,
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                var expectedResponsePayloadBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes("dummy response"));

                spyDataStore.Responses.Should()
                            .Contain(r => r.ResponsePayloadBase64.Equals(expectedResponsePayloadBase64));
            }
        }

        [Fact]
        public async void Middleware_WhenMockingIsEnabled_VerifiesTheCallIsMade()
        {
            var spyDataStore = new SpyDataStore();
            var resource = "test";

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                                                                     .Build(),
                                                             spyDataStore)
                    .Build())
            {
                await testServer
                        .CreateRequest(resource)
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                var verificationResponse = await testServer
                                                   .CreateRequest($"mocks/verify")
                                                   .And(msg =>
                                                   {
                                                       msg.Content = new StringContent("{Resource:'test', HttpMethod: 'Post', RequestPayload: 'dummy request'}");
                                                   })
                                                   .GetAsync();

                var verificationResponseObject = JsonConvert
                        .DeserializeObject<VerificationResponse>
                        (verificationResponse.Content.ReadAsStringAsync().Result);

                verificationResponseObject.Resource.Should().Be(resource);
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
                await testServer
                        .CreateRequest("test")
                        .And(message =>
                        {
                            message.Content = new StringContent("dummy request");
                        })
                        .PostAsync();

                var verificationResponse = await testServer
                                                   .CreateRequest("mocks/verify")
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
        public async void Middleware_WithVerificationEnabledImposter_SavesTheRequestAndInvocationCountInDataStore()
        {
            var requestContent = "dummy request";
            var spyDataStore = new SpyDataStore();
            var fakeImposter
                    = new FakeImposterWithMockedRequestContent(HttpMethod.Post)
                            .Build();

            using (var testServer = new TestServerBuilder()

                    .UsingImposterMiddleWareWithSpyDataStore(fakeImposter,
                                                             spyDataStore)
                    .Build())
            {
                for (int i = 0; i < 2; i++)
                {
                    await testServer
                            .CreateRequest($"{fakeImposter.Resource}")
                            .And(message =>
                                 {
                                     message.Content = new StringContent("dummy request");
                                 })
                            .PostAsync();
                }

                spyDataStore.Requests.Should().HaveCount(2);
                Encoding.UTF8.GetString(Convert.FromBase64String(spyDataStore.Requests.First().RequestPayloadBase64))
                        .Should().Be(requestContent);
            }
        }
    }
}
