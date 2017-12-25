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
        private readonly ExpressionBuilder _expressionBuilder;
        public ExpressionData<T> ExpressionData { get; }

        public LeftConditionExpression(ExpressionBuilder expressionBuilder)
        {
            _expressionBuilder = expressionBuilder;
            ExpressionData = new ExpressionData<T>();
        }
        public ILogicalConditionExpression<T> IsTrue(Expression<Func<T, bool>> expression)
        {
            var conditionExpression = new ConditionExpression<T>()
                                      {
                                          LeftConditionExpression = expression,
                                      };
            ExpressionData.ConditionExpressions.Add(conditionExpression);

            return new LogicalConditionExpression<T>(ExpressionData,
                                                     conditionExpression,
                                                     _expressionBuilder);
        }
    }
}
