using System;
using System.IO;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal static class RulesEvaluator
    {
        public static Response Evaluate(Imposter imposter, Stream requestStream)
        {
            using (var streamReader = new StreamReader(requestStream))
            {
                var content = streamReader.ReadToEnd();
                return EvaluateRules(imposter, content);
            }
        }

        private static Response EvaluateRules(Imposter imposter, string content)
        {
            foreach (var imposterRule in imposter.Rules)
            {
                var condition = imposterRule.Condition.Compile();

                if (ConditionMatches(content, condition))
                {
                    return imposterRule.Action;
                }
            }

            return CreateInternalServerErrorResponse();
        }

        private static Response CreateInternalServerErrorResponse()
        {
            return new Response()
                   {
                       Content = "None of the imposter conditions matched.",
                       StatusCode = 500
                   };
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
