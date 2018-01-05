using System;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal class HeaderEvaluator: IEvaluator
    {
        public EvaluationResult Evaluate(Imposter imposter, Request request)
        {
            foreach (var imposterRule in imposter.Rules)
            {
                var condition = imposterRule.Condition.Compile();

                if (ConditionMatches(request.RequestHeader, condition))
                {
                    return new EvaluationResult(RuleEvaluationOutcome.FoundAMatch,
                                                imposterRule.ResponseCreatorAction.CreateResponse());
                }
            }

            return new EvaluationResult(RuleEvaluationOutcome.NoMatchesFound);
        }

        private static bool ConditionMatches(RequestHeader requestHeader, Func<Request, bool> condition)
        {
            return condition(BuildRequestUsing(requestHeader));
        }

        private static Request BuildRequestUsing(RequestHeader requestHeader)
        {
            return new Request()
                   {
                       RequestHeader = requestHeader
                   };
        }
    }
}
