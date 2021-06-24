using System;
using System.Linq.Expressions;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Http;

namespace FluentImposter.AspnetCore
{
    public class ImposterRulesEvaluator
    {
        public (Response response, Expression<Func<Request, bool>> matchedCondition)
                EvaluateRules(RestImposter imposter,
                              HttpContext context,
                              Request request)
        {
            var response = RulesEvaluator.Evaluate(imposter,
                                           request,
                                           out Expression<Func<Request, bool>> matchedCondition);

            return (response, matchedCondition);
        }
    }
}
