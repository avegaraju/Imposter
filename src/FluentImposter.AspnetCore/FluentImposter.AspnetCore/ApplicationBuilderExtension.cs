using System;
using System.IO;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        public static void UseImposters(this IApplicationBuilder applicationBuilder,
                                        Uri baseUri,
                                        Imposter[] imposters)
        {
            MapHandlers(imposters,applicationBuilder);
        }

        private static void MapHandlers(Imposter[] imposters, IApplicationBuilder applicationBuilder)
        {
            foreach (var imposter in imposters)
            {
                applicationBuilder.Map(imposter.Resource,
                                       app => HandleRequest(app, imposter));
            }
        }

        private static void HandleRequest(IApplicationBuilder applicationBuilder, Imposter imposter)
        {
            applicationBuilder.Run(async context =>
                                   {
                                       var streamReader = new StreamReader(context.Request.Body);
                                       var content = streamReader.ReadToEnd();

                                       foreach (var imposterRule in imposter.Rules)
                                       {
                                           var condition = imposterRule.Condition.Compile();

                                           if (condition(new Request()
                                                         {
                                                             Body = new Body()
                                                                    {
                                                                        Content = content
                                                                    }
                                                         }))
                                           {
                                               await context.Response.WriteAsync(imposterRule.Action.Body.Content);
                                           }
                                       }
                                   });
        }
    }
}
