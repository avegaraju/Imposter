using System;
using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal class RuleBuilder<T,R> where R : IActionExecutor
    {
        public RuleBuilder()
        {
            RuleData = new RuleData<T,R>();
        }
        public RuleData<T,R> RuleData { get; }
        public IRuleAction<T,R> When(Expression<Func<T, bool>> condition)
        {
            RuleData.Condition = condition ?? throw new ArgumentNullException(nameof(condition));

            return new RuleAction<T,R>(this);
        }
    }
}
