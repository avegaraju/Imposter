using System;
using System.Net;
using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FluentImposter.AspnetCore.Tests.Integration
{
    internal class TestServerBuilder: IDisposable
    {
        private TestServer _testServer;
        private readonly WebHostBuilder _webHostBuilder;

        public TestServerBuilder()
        {
            _webHostBuilder = new WebHostBuilder();
            _webHostBuilder.ConfigureServices(sc => sc.AddRouting());
        }

        public TestServer Build()
        {
            _testServer = new TestServer(_webHostBuilder);

            return _testServer;
        }

        public TestServerBuilder UsingImpostersMiddleware(RestImposter imposter)
        {
            _webHostBuilder.Configure(builder => builder.UseStubImposters(new[] { imposter }));

            return this;
        }

        public TestServerBuilder UsingImposterMiddleWareWithSpyDataStore(RestImposter imposter,
                                                                            IDataStore spyDataStore)
        {
            _webHostBuilder.Configure(
                builder => builder.UseMockImposters(
                    new[] {imposter},
                    spyDataStore
                )
            );

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
            return new ResponseBuilder()
                    .WithContent("dummy response")
                    .WithStatusCode(HttpStatusCode.OK)
                    .Build();
        }
    }
}
