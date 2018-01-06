using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal static class RulesEvaluator
    {
        public static Response Evaluate(Imposter imposter, Request request)
        {
            foreach (var imposterRule in imposter.Rules)
            {
                var condition = imposterRule.Condition.Compile();

                if (condition(request))
                {
                    return imposterRule.ResponseCreatorAction.CreateResponse();
                }
            }

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
