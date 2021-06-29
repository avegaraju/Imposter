using FluentImposter.AspnetCore;
using ImpostersServiceSample.Stubs;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImpostersServiceSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStubImposters(
                new StubImpostersBuilder().CreateStubImposters()
            );

            /*Uncomment below code when DynamoDB is available at http://localhost:8000*/

            //app.UseMockImposters(
            //    new MockImpostersBuilder().CreateMockImposters(),
            //    new MocksDataStore().Create()
            //);

            app.UseMvc();
        }
    }
}
