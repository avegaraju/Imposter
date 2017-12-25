using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal  interface ILogicalConditionExpression<T>
    {
        ExpressionData<T> ExpressionData { get; }
        IRightConditionExpression And();
    }

    internal class LogicalConditionExpression<T>: ILogicalConditionExpression<T>
    {
        public LogicalConditionExpression(ExpressionData<T> expressionData)
        {
            ExpressionData = expressionData;
        }

        public ExpressionData<T> ExpressionData { get; }
        public IRightConditionExpression And()
        {
            throw new System.NotImplementedException();
        }
    }
}
