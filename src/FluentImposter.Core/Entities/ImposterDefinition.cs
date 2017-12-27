﻿using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public enum ImposterType
    {
        REST,
        SOAP
    }
    public class ImposterDefinition
    {
        private readonly Imposter _imposter;

        public ImposterDefinition(Imposter imposter)
        {
            _imposter = imposter;
        }
        public ImposterRuleBuilder IsOfType(ImposterType type)
        {
            _imposter.SetType(type);

            return new ImposterRuleBuilder(_imposter);
        }
    }
}