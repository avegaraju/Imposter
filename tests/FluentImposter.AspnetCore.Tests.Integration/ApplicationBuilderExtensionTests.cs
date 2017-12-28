using System;
using System.Net.Http;

using FluentAssertions;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

using Xunit;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    public class ApplicationBuilderExtensionTests
    {
        private Action<IApplicationBuilder> _applicationBuilder;

        [Fact]
        public async void Middleware_RouteHandler_InvokesHandlerAsPerResource()
        {
            var webHostBuilder = new WebHostBuilder()
                    .Configure(_applicationBuilder);

            _applicationBuilder = app =>
                                  {
                                      app.UseImposters(new Uri("http://localhost:8080"),
                                                       new Imposter[]
                                                       {
                                                           new DummyImposter().Build(),
                                                       });
                                  };


            using (var testServer = new TestServer(webHostBuilder))
            {
                var response = await testServer
                                       .CreateRequest("/")
                                       .And(message =>
                                            {
                                                message.Content =
                                                        new StringContent("yo");
                                            })
                                       .PostAsync();

                response.Content.ToString()
                        .Should().Contain("yoyo");
            }
        }
    }

    internal class DummyImposter: IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .IsOfType(ImposterType.REST)
                    .StubsResource("/test")
                    .When(r => r.Body.Content.Contains("yo"))
                    .Then(a => new DummyResponseCreator().CreateResponse())
                    .Build();
        }
    }

    internal class DummyResponseCreator: IResponseCreator
    {
        public Response CreateResponse()
        {
            return new Response()
                   {
                       Body = new Body()
                              {
                                  Content = "yoyo"
                              }
                   };
        }
    }
}
