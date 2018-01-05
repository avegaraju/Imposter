using System;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal class BodyEvaluator: IEvaluator
    {
        public EvaluationResult Evaluate(Imposter imposter, Request request)
        {
            foreach (var imposterRule in imposter.Rules)
            {
                var condition = imposterRule.Condition.Compile();

                if (ConditionMatches(request.Content, condition))
                {
                    return new EvaluationResult(RuleEvaluationOutcome.FoundAMatch,
                                                imposterRule.ResponseCreatorAction.CreateResponse());
                }
            }

            return new EvaluationResult(RuleEvaluationOutcome.NoMatchesFound);
        }
        
        private static bool ConditionMatches(string content, Func<Request, bool> condition)
        {
            return condition(BuildRequestUsing(content));
        }

        private static Request BuildRequestUsing(string content)
        {
            return new Request()
                   {
                       Content = content
                   };
        }
    }
}
