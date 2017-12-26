using System;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine.Entities
{
    internal class RuleData<T,R> where R: IActionExecutor
    {
        public Expression<Func<T,bool>> Condition { get; set; }
        public Expression<Action<R>> ActionExecutor { get; set; }
    }
}
