using System;
using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    public interface IRuleAction<T,R,U> where R: IActionExecutor<U>
    {
        IBuilder<T,R,U> Then(Expression<Action<R>> actionExecutor);
    }

    internal class RuleAction<T,R,U>: IRuleAction<T,R,U> where R: IActionExecutor<U> where U: new()
    {
        private readonly RuleBuilder<T,R,U> _ruleBuilder;

        public RuleAction(RuleBuilder<T,R,U> ruleBuilder)
        {
            _ruleBuilder = ruleBuilder;
        }

        public IBuilder<T,R,U> Then(Expression<Action<R>> actionExecutor)
        {
            actionExecutor = actionExecutor ?? throw new ArgumentNullException(nameof(actionExecutor));

            return new Builder<T,R,U>(_ruleBuilder, actionExecutor);
        }
    }
}
