using System;
using System.Collections.Generic;
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
        public void When_IsTrue_AddsLeftConditionExpressionToExpressionData()
        {
            var expressionBuilder = new ExpressionBuilder();
            var leftConditionExpression = expressionBuilder
                    .When<Request>()
                    .IsTrue(request => request.Body.Content.Contains("")
                                       && request.Body.Content.Contains(""));

            Expression<Func<Request, bool>> expectedExpression
                    = request => request.Body.Content.Contains("")
                                 && request.Body.Content.Contains("");

            leftConditionExpression.ExpressionData
                                   .LeftConditionExpressions.First()
                                   .Should().BeEquivalentTo(expectedExpression);
        }

        [Fact]
        public void When_IsTrue_And_SetsExpressionTypeToAndInExpressionData()
        {
            var expressionBuilder = new ExpressionBuilder();
            var rightConditionExpression = expressionBuilder
                    .When<Request>()
                    .IsTrue(request => request.Body.Content.Contains("")
                                       && request.Body.Content.Contains(""))
                    .And;

            rightConditionExpression.ExpressionData
                                    .ConditionExpressionType.Should().Be(ExpressionType.And);
        }

        [Fact]
        public void When_IsTrue_And_AddsRightConditionExpressionToExpressionData()
        {
            var expressionBuilder = new ExpressionBuilder();
            var rightConditionExpression = expressionBuilder
                    .When<Request>()
                    .IsTrue(request => request.Body.Content.Contains("")
                                       && request.Body.Content.Contains(""))
                    .And
                    .IsTrue(request => request.Body.Content.Contains(""));

            Expression<Func<Request, bool>> expectedExpression
                    = request => request.Body.Content.Contains("");

            rightConditionExpression.ExpressionData
                                    .RightConditionExpression.First()
                                    .Should().BeEquivalentTo(expectedExpression);
        }

        [Fact]
        public void ExpressionEvaluator_Evaluates_ExpressionDataCorrectly()
        {
            
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
