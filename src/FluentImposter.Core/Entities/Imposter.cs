using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentImposter.Core.Entities
{
    public class Imposter
    {
        private Rule _newRule;
        private readonly List<Rule> _rules;

        public string Name { get; }
        public string Resource { get; private set; }
        public ImposterType Type { get; private set; }
        public IEnumerable<Rule> Rules => _rules;

        internal Imposter(string name)
        {
            Name = name;
            _rules = new List<Rule>();
        }

        internal void SetType(ImposterType type)
        {
            Type = type;
        }

        internal void CreateRuleCondition(Expression<Func<Request,bool>> condition)
        {
            _newRule = new Rule();
            _newRule.SetCondition(condition);
            
            _rules.Add(_newRule);
        }
        
        internal void CreateRuleAction(Response action)
        {
            _newRule.SetAction(action);
        }

        internal void SetResource(string resourcePath)
        {
            Resource = resourcePath;
        }
    }
}
