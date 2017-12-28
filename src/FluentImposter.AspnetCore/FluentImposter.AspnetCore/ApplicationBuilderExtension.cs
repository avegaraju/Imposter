using System;

using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        public static void UseImposters(this IApplicationBuilder app,
                                        Uri baseUri,
                                        Imposter[] imposters)
        {
            MapHandlers(imposters);
        }

        private static void MapHandlers(Imposter[] imposters)
        {
            foreach (var imposter in imposters)
            {
                
            }
        }
    }
}
