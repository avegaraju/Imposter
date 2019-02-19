using System;

using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FluentImposter.AspnetCore
{
    public class RouteCreatorBase
    {
        protected void CreateImposterResourceRoutes(IApplicationBuilder applicationBuilder,
                                                    Imposter[] imposters,
                                                    Func<Imposter,RequestDelegate> rulesEvaluator)
        {
            foreach (var imposter in imposters)
            {
                applicationBuilder
                        .UseRouter(routeBuilder =>
                                   {
                                       routeBuilder.MapVerb(imposter.Method.ToString(),
                                                            imposter.Resource,
                                                            rulesEvaluator(imposter));
                                   });
            }
        }
    }
}
