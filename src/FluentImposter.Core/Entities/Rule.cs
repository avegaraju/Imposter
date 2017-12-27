using System;
using System.Linq.Expressions;

namespace FluentImposter.Core.Entities
{
    public class Rule
    {
        public Expression<Func<Request, bool>> Condition { get; private set; }
        public Expression<Action<IResponseCreator>> Action { get; private set; }

        internal void SetCondition(Expression<Func<Request, bool>> condition)
        {
            Condition = condition;
        }
        internal void SetAction(Expression<Action<IResponseCreator>> action)
        {
            Action = action;
        }
    }
}
