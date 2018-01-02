using System;
using System.IO;
using System.Threading.Tasks;

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
            applicationBuilder.Run(EvaluateImposterRules(imposter));
        }

        private static RequestDelegate EvaluateImposterRules(Imposter imposter)
        {
            return async context =>
            {
                using (var streamReader = new StreamReader(context.Request.Body))
                {
                    var content = streamReader.ReadToEnd();

                    await EvaluateRules(imposter, context, content);
                }
            };
        }

        private static async Task EvaluateRules(Imposter imposter, HttpContext context, string content)
        {
            var response = RulesEvaluator.Evaluate(imposter, context.Request.Body);

            await context.Response.WriteAsync(response.Content);
        }
    }
}
