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

        public TestServerBuilder UsingImpostersMiddleware(Imposter imposter)
        {
            Action<IApplicationBuilder> action = app => app.UseImposters(new Uri("http://localhost:8080"),
                                             new[]
                                             {
                                                 imposter,
                                             });

            _webHostBuilder.Configure(action);

            return this;
        }

        public void Dispose()
        {
            _testServer?.Dispose();
        }
    }

    internal class DummyResponseCreator : IResponseCreator
    {
        public Response CreateResponse()
        {
            return new Response()
                   {
                       Content = "dummy response",
                       StatusCode = 200
                   };
        }
    }
}
