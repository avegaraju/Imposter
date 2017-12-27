using System;
using System.Linq.Expressions;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterRuleBuilder
    {
        private readonly Imposter _imposter;

        public ImposterRuleBuilder(Imposter imposter)
        {
            _imposter = imposter;
        }
        public ImposterRuleAction When(Expression<Func<Request, bool>> condition)
        {
            _imposter.SetCondition(condition);
            return new ImposterRuleAction(_imposter);
        }
    }
}
