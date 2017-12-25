using System;
using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal interface IRuleAction<T,R> where R: IActionExecutor
    {
        IBuilder<T,R> Then(Expression<Action<R>> actionExecutor);
    }

    internal class RuleAction<T,R>: IRuleAction<T,R> where R: IActionExecutor
    {
        private readonly RuleBuilder<T,R> _ruleBuilder;

        public RuleAction(RuleBuilder<T,R> ruleBuilder)
        {
            _ruleBuilder = ruleBuilder;
        }

        public IBuilder<T,R> Then(Expression<Action<R>> actionExecutor)
        {
            actionExecutor = actionExecutor ?? throw new ArgumentNullException(nameof(actionExecutor));

            return new Builder<T,R>(_ruleBuilder, actionExecutor);
        }
    }
}
