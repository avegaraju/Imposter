using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    internal interface IEvaluator
    {
        EvaluationResult Evaluate(Imposter imposter, Request request);
    }
}
