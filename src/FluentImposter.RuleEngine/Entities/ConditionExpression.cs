using System;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine.Entities
{
    internal class ConditionExpression<T>
    {
        public Expression<Func<T, bool>> LeftConditionExpression { get; set; }
        public Expression<Func<T, bool>> RightConditionExpression { get; set; }
        public ExpressionType ConditionExpressionType { get; set; }
    }
}
