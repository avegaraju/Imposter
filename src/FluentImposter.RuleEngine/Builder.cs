using System;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine
{
    public interface IBuilder<T,R,U> where R: IActionExecutor<U>  
    {
        void Build();
    }
    public class Builder<T,R, U>: IBuilder<T,R, U> where R: IActionExecutor<U> where U : new()
    {
        private readonly RuleBuilder<T,R,U> _ruleBuilder;
        private readonly Expression<Action<R>> _actionExcutor;

        public Builder(RuleBuilder<T, R,U> ruleBuilder,
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
