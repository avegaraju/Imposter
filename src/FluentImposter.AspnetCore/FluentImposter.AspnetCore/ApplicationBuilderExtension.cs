using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        public static void UseImposters(this IApplicationBuilder applicationBuilder,
                                        ImposterConfiguration imposterConfiguration)
        {
            MapHandlers(imposterConfiguration.Imposters, applicationBuilder);
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
            var request = BuildRequest(context);

            var response = RulesEvaluator.Evaluate(imposter, request);

            await context.Response.WriteAsync(response.Content);
        }

        private static Request BuildRequest(HttpContext context)
        {
            var stream = context.Request.Body;
            stream.Position = 0;

            using (var streamReader = new StreamReader(stream))
            {
                return new Request()
                       {
                           Content = streamReader.ReadToEnd(),
                           RequestHeader = BuildRequestHeader()
                       };
            }

            RequestHeader BuildRequestHeader()
            {
                var requestHeader = new RequestHeader();

                foreach (KeyValuePair<string, StringValues> keyValuePair in context.Request.Headers)
                {
                    requestHeader.Add(keyValuePair.Key, keyValuePair.Value);
                }

                return requestHeader;
            }
        }
    }
}
