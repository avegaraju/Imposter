using System;
using System.Linq;
using System.Linq.Expressions;

using FluentAssertions;

using Xunit;

namespace FluentImposter.RuleEngine.Tests
{
    public class ExpressionBuilderTests
    {
        [Fact]
        public void When_ReturnsILeftConditionExpression()
        {
            var expressionBuilder = new ExpressionBuilder();

            expressionBuilder.When<Request>()
                             .Should().BeAssignableTo<ILeftConditionExpression<Request>>();
        }

        [Fact]
        public void When_IsTrue_AllowsToAddExpressionToExpressionData()
        {
            Expression<Func<Request, bool>> expectedExpression
                    = request => request.Body.Content.Contains("")
                                 && request.Body.Content.Contains("");


            var expressionBuilder = new ExpressionBuilder();
            var leftConditionExpression = expressionBuilder
                    .When<Request>()
                    .IsTrue(expectedExpression);


            leftConditionExpression.ExpressionData
                                   .LeftConditionExpressions.First()
                                   .Should().Be(expectedExpression);
        }
    }

    internal class Request
    {
        public Body Body { get; set; }   
    }

    internal class Body
    {
        public string Content { get; set; }
    }
}
