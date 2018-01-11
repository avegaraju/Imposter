using System;
using System.Collections.Generic;
using System.Text;

using FluentImposter.AspnetCore;
using FluentImposter.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SampleClientService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var userIdentityImposter = new UserIdentityImposter().Build();

            var imposterConfiguration = new ImposterConfiguration(new[]
                                                                  {
                                                                      userIdentityImposter
                                                                  });
            app.UseImposters(imposterConfiguration);
        }
    }
}
