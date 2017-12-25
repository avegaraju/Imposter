using System.Linq.Expressions;

using FluentImposter.RuleEngine;
using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal  interface ILogicalConditionExpression<T>
    {
        ExpressionData<T> ExpressionData { get; }
        IRightConditionExpression<T> And { get; }
    }

    internal class LogicalConditionExpression<T>: ILogicalConditionExpression<T>
    {
        private readonly ExpressionType _expressionBuilderExpressionType;
        private readonly ConditionExpression<T> _conditionExpression;

        public LogicalConditionExpression(ExpressionData<T> expressionData,
                                          ConditionExpression<T> conditionExpression,
                                          ExpressionType expressionBuilderExpressionType)
        {
            _expressionBuilderExpressionType = expressionBuilderExpressionType;
            _conditionExpression = conditionExpression ?? new ConditionExpression<T>();
            ExpressionData = expressionData;
        }

        public ExpressionData<T> ExpressionData { get; }

        public IRightConditionExpression<T> And
        {
            get
            {
                _conditionExpression.ConditionExpressionType = ExpressionType.And;

                return new RightConditionExpression<T>(ExpressionData,
                                                       _conditionExpression,
                                                       _expressionBuilder);
            }
        }
    }
}
