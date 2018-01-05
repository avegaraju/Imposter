using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal class EvaluationResult
    {
        public RuleEvaluationOutcome Outcome { get; }
        public Response Response { get; }

        public EvaluationResult(RuleEvaluationOutcome outcome, Response response)
        {
            Outcome = outcome;
            Response = response;
        }

        public EvaluationResult(RuleEvaluationOutcome outcome)
        {
            Outcome = outcome;
        }
    }

    internal enum RuleEvaluationOutcome
    {
        NoMatchesFound,
        FoundAMatch
    }
}
