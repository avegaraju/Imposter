using System.Linq.Expressions;

using FluentImposter.RuleEngine.Entities;

namespace FluentImposter.RuleEngine
{
    internal class ExpressionEvaluator<T>
    {
        private readonly ExpressionData<T> _expressionData;

        public ExpressionEvaluator(ExpressionData<T> expressionData)
        {
            _expressionData = expressionData;
        }

        //public bool Evaluate()
        //{
        //    var expression = Expression.Lambda<T>(Expression.And(_expressionData.LeftConditionExpressions))

        //}
    }
}
