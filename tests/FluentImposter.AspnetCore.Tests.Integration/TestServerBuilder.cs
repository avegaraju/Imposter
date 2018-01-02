using System;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    internal class TestServerBuilder: IDisposable
    {
        private TestServer _testServer;
        private readonly WebHostBuilder _webHostBuilder;

        public TestServerBuilder()
        {
            _webHostBuilder = new WebHostBuilder();
        }

        public TestServer Build()
        {
            _testServer = new TestServer(_webHostBuilder);

            return _testServer;
        }

        public TestServerBuilder UsingImpostersMiddleware()
        {
            Action<IApplicationBuilder> action = app => app.UseImposters(new Uri("http://localhost:8080"),
                                             new Imposter[]
                                             {
                                                 new DummyImposter().Build(),
                                             });

            _webHostBuilder.Configure(action);

            return this;
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }

    internal class DummyImposter : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .IsOfType(ImposterType.REST)
                    .StubsResource("/test")
                    .When(r => r.Content.Contains("dummy"))
                    .Then(new DummyResponseCreator().CreateResponse())
                    .Build();
        }
    }

    internal class DummyResponseCreator : IResponseCreator
    {
        public Response CreateResponse()
        {
            return new Response()
                   {
                       Content = "dummy response"
                   };
        }
    }
}
