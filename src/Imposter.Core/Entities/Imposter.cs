using System;
using System.Linq.Expressions;

using FluentImposter.Core.DSL;

namespace FluentImposter.Core.Entities
{
    public class Imposter
    {
        public string Name { get; }
        public ImposterType Type { get; private set; }
        public Expression<Func<Request,bool>> Condition { get; private set; }
        public Expression<Action<IResponseCreator>> Action { get; private set; }

        public Imposter(string name)
        {
            Name = name;
        }

        internal void SetType(ImposterType type)
        {
            Type = type;
        }

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
