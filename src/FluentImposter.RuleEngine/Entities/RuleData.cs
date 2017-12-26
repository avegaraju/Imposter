using System;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine.Entities
{
    public class RuleData<T,R, U> where R: IActionExecutor<U>
    {
        public Expression<Func<T,bool>> Condition { get; set; }
        public Expression<Action<R>> ActionExecutor { get; set; }
    }
}
