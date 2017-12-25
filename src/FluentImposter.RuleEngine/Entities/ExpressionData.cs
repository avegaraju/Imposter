using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine.Entities
{
    internal class ExpressionData<T>
    {
        public ExpressionData()
        {
            LeftConditionExpressions = new List<Expression<Func<T, bool>>>();
        }

        public List<Expression<Func<T,bool>>> LeftConditionExpressions { get; set; } 
    }
}
