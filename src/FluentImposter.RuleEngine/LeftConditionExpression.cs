using System;
using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal interface ILeftConditionExpression<T>
    {
        ExpressionData<T> ExpressionData { get; }
        ILogicalConditionExpression<T> IsTrue(Expression<Func<T, bool>> expression);
    }

    internal class LeftConditionExpression<T>: ILeftConditionExpression<T>
    {
        public ExpressionData<T> ExpressionData { get; }

        public LeftConditionExpression()
        {
            ExpressionData = new ExpressionData<T>();
        }
        public ILogicalConditionExpression<T> IsTrue(Expression<Func<T, bool>> expression)
        {
            ExpressionData.LeftConditionExpressions.Add(expression);

            return new LogicalConditionExpression<T>(ExpressionData);
        }
    }
}
