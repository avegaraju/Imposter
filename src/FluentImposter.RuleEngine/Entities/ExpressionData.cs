using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentImposter.RuleEngine.Entities
{
    internal class ExpressionData<T>
    {
        
        public ExpressionData()
        {
            //LeftConditionExpressions = new List<Expression<Func<T, bool>>>();
            //RightConditionExpression = new List<Expression<Func<T, bool>>>();
        }

        public List<ConditionExpression<T>> ConditionExpressions { get; set; }
        //public List<Expression<Func<T,bool>>> LeftConditionExpressions { get; set; } 
        //public List<Expression<Func<T,bool>>> RightConditionExpression { get; set; }
        public ExpressionType ConditionExpressionType { get; set; }
    }
}
