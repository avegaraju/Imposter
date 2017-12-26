using System;
using System.Linq.Expressions;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.DSL
{
    public class ImposterRuleAction
    {
        private readonly Imposter _imposter;

        public ImposterRuleAction(Imposter imposter)
        {
            _imposter = imposter;
        }
        public IBuilder Then(Expression<Action<ResponseCreator>> actionExecutor)
        {
            _imposter.SetAction(actionExecutor);

            return new Builder(_imposter);
        }
    }
}
