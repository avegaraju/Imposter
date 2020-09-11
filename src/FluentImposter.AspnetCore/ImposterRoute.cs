using System;

using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FluentImposter.AspnetCore
{
    public interface IImposterRoute
    {
        void CreateImposterResourceRoutes(
            IApplicationBuilder applicationBuilder,
            Imposter[] imposters,
            Func<Imposter, RequestDelegate> rulesEvaluator
        );
    }

    public class ImposterRoute: IImposterRoute
    {
        public void CreateImposterResourceRoutes(IApplicationBuilder applicationBuilder,
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
