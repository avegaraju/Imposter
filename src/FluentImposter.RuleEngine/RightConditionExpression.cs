using System;
using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal interface IRightConditionExpression<T>
    {
        ExpressionData<T> ExpressionData { get; }

        ILogicalConditionExpression<T> IsTrue(Expression<Func<T, bool>> expression);
    }

    internal class RightConditionExpression<T>: IRightConditionExpression<T>
    {
        private readonly ConditionExpression<T> _conditionExpression;
        private readonly ExpressionBuilder _expressionBuilder;

        public RightConditionExpression(ExpressionData<T> expressionData,
                                        ConditionExpression<T> conditionExpression,
                                        ExpressionBuilder expressionBuilder)
        {
            _conditionExpression = conditionExpression;
            _expressionBuilder = expressionBuilder;
            ExpressionData = expressionData;
        }

        public ILogicalConditionExpression<T> IsTrue(Expression<Func<T, bool>> expression)
        {
            _conditionExpression.RightConditionExpression = expression;

            return new LogicalConditionExpression<T>(ExpressionData, null, );
        }

        public ExpressionData<T> ExpressionData { get; }
    }
}
