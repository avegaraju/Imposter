using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;

namespace FluentImposter.Core.Entities
{
    public class RestImposter: IImposter
    {
        private Rule _newRule;
        private readonly List<Rule> _rules;

        public string Name { get; }
        public string Resource { get; private set; }
        public HttpMethod Method { get; private set; }
        public IEnumerable<Rule> Rules => _rules;

        internal RestImposter(string name)
        {
            Name = name;
            _rules = new List<Rule>();
        }

        internal void CreateRuleCondition(Expression<Func<Request,bool>> condition)
        {
            _newRule = new Rule();
            _newRule.SetCondition(condition);
            
            _rules.Add(_newRule);
        }

        internal void CreateRuleAction(IResponseCreator responseCreator)
        {
            _newRule.SetAction(responseCreator);
        }

        internal void SetResource(string resourcePath)
        {
            Resource = resourcePath;
        }

        internal void SetMethod(HttpMethod method)
        {
            Method = method;
        }
    }
}
