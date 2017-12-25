using System;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine
{
    internal interface IBuilder<T,R> where R: IActionExecutor
    {
        void Build();
    }
    internal class Builder<T,R>: IBuilder<T,R> where R: IActionExecutor
    {
        private readonly RuleBuilder<T,R> _ruleBuilder;
        private readonly Expression<Action<R>> _actionExcutor;

        public Builder(RuleBuilder<T, R> ruleBuilder,
                       Expression<Action<R>> actionExcutor)
        {
            _ruleBuilder = ruleBuilder;
            _actionExcutor = actionExcutor;
        }

        public void Build()
        {
            _ruleBuilder.RuleData.ActionExecutor = _actionExcutor;
        }
    }
}
