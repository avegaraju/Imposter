using System;
using System.Linq.Expressions;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterRuleAction
    {
        private readonly Imposter _imposter;

        public ImposterRuleAction(Imposter imposter)
        {
            _imposter = imposter;
        }
        public ImposterRule Then(Expression<Action<IResponseCreator>> actionExecutor)
        {
            _imposter.CreateRuleAction(actionExecutor);

            return new ImposterRule(_imposter);
        }
    }
}
