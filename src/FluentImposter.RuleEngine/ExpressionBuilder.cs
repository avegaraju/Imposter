namespace FluentImposter.RuleEngine
{
    internal class ExpressionBuilder
    {
        public ILeftConditionExpression<T> When<T>()
        {
            return new LeftConditionExpression<T>(this);
        }
    }
}
