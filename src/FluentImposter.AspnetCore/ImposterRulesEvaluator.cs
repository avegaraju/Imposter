using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace FluentImposter.AspnetCore
{
    public class ImposterRulesEvaluator
    {
        public (Response response, Expression<Func<Request, bool>> matchedCondition)
                EvaluateRules(Imposter imposter,
                              HttpContext context)
        {
            var request = BuildRequest(context);

            var response = RulesEvaluator.Evaluate(imposter,
                                           request,
                                           out Expression<Func<Request, bool>> matchedCondition);

            return (response, matchedCondition);
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
                                  SessionId = _currentSession
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
