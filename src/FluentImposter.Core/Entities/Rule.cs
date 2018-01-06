using System;
using System.Linq.Expressions;

namespace FluentImposter.Core.Entities
{
    public class Rule
    {
        public Expression<Func<Request, bool>> Condition { get; private set; }
        public IResponseCreator ResponseCreatorAction { get; private set; }

        internal void SetCondition(Expression<Func<Request, bool>> condition)
        {
            Condition = condition;
        }

        internal void SetAction(IResponseCreator responseCreator)
        {
            ResponseCreatorAction = responseCreator;
        }
    }
}
