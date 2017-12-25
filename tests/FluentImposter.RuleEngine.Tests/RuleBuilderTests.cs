using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using FluentAssertions;

using Xunit;

namespace FluentImposter.RuleEngine.Tests
{
    public class RuleBuilderTests
    {
        [Fact]
        public void  When_AddsCorrectContionToRuleData()
        {
            var ruleBuilder = new RuleBuilder<Request,ActionExecutor>();

            ruleBuilder.When(r => r.Body.Content.Contains(""))
                       .Then(a => a.Execute())
                       .Build();

            Expression<Func<Request, bool>> expectedCondition
                    = r => r.Body.Content.Contains("");

            ruleBuilder.RuleData.Condition
                       .Should().BeEquivalentTo(expectedCondition);
        }

        [Fact]
        public void When_WithNullCondition_ThrowsArgumentNullException()
        {
            var ruleBuilder = new RuleBuilder<Request, ActionExecutor>();

            Action exceptionAction = () => ruleBuilder.When(null)
                                                      .Then(a => a.Execute())
                                                      .Build();

            exceptionAction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Then_WithNullActionThrowsArgumentNullException()
        {
            var ruleBuilder = new RuleBuilder<Request, ActionExecutor>();

            Action exceptionAction = () => ruleBuilder.When(r => r.Body.Content.Contains(""))
                                                      .Then(null)
                                                      .Build();

            exceptionAction.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void Then_WithValidActionAddsTheActionToRuleData()
        {
            var ruleBuilder = new RuleBuilder<Request, ActionExecutor>();

            IActionExecutor actionExecutor = new ActionExecutor();

            ruleBuilder.When(r => r.Body.Content.Contains(""))
                       .Then(a => actionExecutor.Execute())
                       .Build();

            Expression<Action<ActionExecutor>> action = a => a.Execute();

            ruleBuilder.RuleData.ActionExecutor.Should().BeEquivalentTo(action);
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

    internal class ActionExecutor: IActionExecutor
    {
        public void Execute()
        {
            
        }
    }
}
