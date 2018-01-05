using System;
using System.Linq;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal class RulesEvaluator
    {
        private const int INTERNAL_SERVER_ERROR = 500;
        private readonly IEvaluator[] _evaluators;

        public RulesEvaluator(IEvaluator[] evaluators)
        {
            _evaluators = evaluators ?? throw new ArgumentNullException(nameof(evaluators));

            if(evaluators.Contains(null))
                throw new ArgumentNullException("One of the evaluators is null.");
        }

        public Response Evaluate(Imposter imposter, Request request)
        {
            foreach (var evaluator in _evaluators)
            {
                var evaluationResult = evaluator.Evaluate(imposter, request);

                if (evaluationResult.Outcome == RuleEvaluationOutcome.FoundAMatch)
                    return evaluationResult.Response;
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
