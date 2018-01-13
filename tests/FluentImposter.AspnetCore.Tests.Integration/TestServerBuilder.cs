﻿using System;

using FluentImposter.AspnetCore.Tests.Integration.Fakes;
using FluentImposter.AspnetCore.Tests.Integration.Spies;
using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
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

        public TestServerBuilder UsingImpostersMiddleware(Imposter imposter)
        {
            var imposterConfiguration = new ImposterConfiguration(new[]
                                                                  {
                                                                      imposter
                                                                  });

            Action<IApplicationBuilder> action =
                    app => app.UseImposters(imposterConfiguration);

            _webHostBuilder.Configure(action);

            return this;
        }

        public TestServerBuilder UsingImposterMiddleWareWithSpyDataStore(Imposter imposter,
                                                                            IDataStore spyDataStore)
        {
            var imposterConfiguration = new ImposterConfiguration(new[]
                                                                  {
                                                                      imposter
                                                                  })
                    .UseSpyDataStore(spyDataStore);

            Action<IApplicationBuilder> action =
                    app => app.UseImposters(imposterConfiguration);

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
            return new ResponseBuilder()
                    .WithContent("dummy response")
                    .WithStatusCode(200)
                    .Build();
        }
    }
}
