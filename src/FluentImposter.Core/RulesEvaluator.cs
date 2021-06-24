using System;
using System.Linq.Expressions;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal static class RulesEvaluator
    {
        public static Response Evaluate(RestImposter imposter, Request request, out Expression<Func<Request, bool>> matchedCondition)
        {
            foreach (var imposterRule in imposter.Rules)
            {
                var condition = imposterRule.Condition.Compile();

                if (condition(request))
                {
                    matchedCondition = imposterRule.Condition;
                    return imposterRule.ResponseCreatorAction.CreateResponse();
                }
            }

            matchedCondition = null;
            return CreateInternalServerErrorResponse();
        }

        private static Response CreateInternalServerErrorResponse()
        {
            return new Response()
                   {
                       Content = "None of evaluators could create a response.",
                       StatusCode = 500
                   };
        }
    }
}
