using FluentImposter.Core;

using Microsoft.AspNetCore.Builder;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        public static void UseMockImposters(this IApplicationBuilder applicationBuilder,
                                            ImpostersAsMockConfiguration impostersAsMockConfiguration)
        {
            var mockingRouteCreator =
                    new MockingRouteCreator(impostersAsMockConfiguration,
                                            new ImposterRulesEvaluator());

            mockingRouteCreator.CreateRoutes(applicationBuilder);

        }

        public static void UseStubImposters(this IApplicationBuilder applicationBuilder,
                                            ImpostersAsStubConfiguration impostersAsStubConfiguration)
        {
            var stubbingRouteCreator =
                    new StubbingRouteCreator(impostersAsStubConfiguration,
                                             new ImposterRulesEvaluator());

            stubbingRouteCreator.CreateRoutes(applicationBuilder);
        }
    }
}
