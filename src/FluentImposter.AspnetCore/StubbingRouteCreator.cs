using System.Threading.Tasks;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FluentImposter.AspnetCore
{
    public class StubbingRouteCreator: IRouteCreator<IApplicationBuilder>
    {
        private readonly ImpostersAsStubConfiguration _configuration;
        private readonly ImposterRulesEvaluator _rulesEvaluator;

        public StubbingRouteCreator(ImpostersAsStubConfiguration configuration,
                                    ImposterRulesEvaluator rulesEvaluator)
        {
            _configuration = configuration;
            _rulesEvaluator = rulesEvaluator;
        }

        public void CreateRoutes(IApplicationBuilder applicationBuilder)
        {
            foreach (var imposter in _configuration.Imposters)
            {
                applicationBuilder
                        .UseRouter(routeBuilder =>
                                   {
                                       routeBuilder.MapVerb(imposter.Method.ToString(),
                                                            imposter.Resource,
                                                            EvaluateImposterRules(imposter));
                                   });
            }

            RequestDelegate EvaluateImposterRules(Imposter imposter)
            {
                return async context =>
                       {
                           await EvaluateRules(imposter, context);
                       };
            }

            async Task EvaluateRules(Imposter imposter,
                                     HttpContext context)
            {
                var (response,_) = _rulesEvaluator.EvaluateRules(imposter, context);

                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync(response.Content);
            }
        }
    }
}
