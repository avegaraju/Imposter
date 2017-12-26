using System;
using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    public class RuleBuilder<T,R, U> where R : IActionExecutor<U> where U: new()
    {
        public RuleBuilder()
        {
            RuleData = new RuleData<T,R,U>();
        }
        public RuleData<T,R,U> RuleData { get; }
        public IRuleAction<T,R,U> When(Expression<Func<T, bool>> condition)
        {
            RuleData.Condition = condition ?? throw new ArgumentNullException(nameof(condition));

            return new RuleAction<T,R,U>(this);
        }
    }
}
