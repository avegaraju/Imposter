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
    public class StubbingRouteCreator: RouteCreatorBase,
                                       IRouteCreator<IApplicationBuilder>
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
            base.CreateImposterResourceRoutes(applicationBuilder,
                                              _configuration.Imposters,
                                              EvaluateImposterRules);

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
                var request = BuildRequest(context);

                var (response, _) = _rulesEvaluator.EvaluateRules(imposter,
                                                                  context,
                                                                  request);

                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync(response.Content);
            }
        }

        private static Request BuildRequest(HttpContext context)
        {
            var stream = context.Request.Body;
            using (var streamReader = new StreamReader(stream))
            {
                var request = new Request()
                              {
                                  Content = streamReader.ReadToEnd(),
                                  RequestHeader = BuildRequestHeader(),
                              };

                return request;
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
