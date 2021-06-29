using FluentImposter.Core;
using FluentImposter.Core.Entities;
using Microsoft.AspNetCore.Builder;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        public static void UseMockImposters(this IApplicationBuilder applicationBuilder,
                                            RestImposter[] restImposters,
                                            IDataStore dataStore)
        {
            var mockingRouteCreator =
                new MockingRouteCreator(
                    restImposters,
                    new ImposterRulesEvaluator(),
                    new ImposterRoute(),
                    dataStore
                );

            mockingRouteCreator.CreateRoutes(applicationBuilder);

        }

        public static void UseStubImposters(this IApplicationBuilder applicationBuilder,
                                            RestImposter[] restImposters)
        {
            var stubbingRouteCreator =
                new StubbingRouteCreator(
                    restImposters,
                    new ImposterRulesEvaluator(),
                    new ImposterRoute()
                );

            stubbingRouteCreator.CreateRoutes(applicationBuilder);
        }
    }
}
