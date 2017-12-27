using System;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public interface IImposterBuilder
    {
        ImposterHostBuilder HasAnImposter(string name,
                                          Action<ImposterDefinition> imposterDefinitionAction);
    }

    public class ImposterBuilder: IImposterBuilder
    {
        private readonly ImposterHostBuilder _imposterHostBuilder;

        public ImposterBuilder(ImposterHostBuilder imposterHostBuilder)
        {
            _imposterHostBuilder = imposterHostBuilder;
        }

        public ImposterHostBuilder HasAnImposter(string name,
                                                 Action<ImposterDefinition> imposterDefinitionAction)
        {
            var imposter = string.IsNullOrEmpty(name)
                               ? new Imposter(Guid.NewGuid().ToString())
                               : new Imposter(name);

            var imposterDefinition = new ImposterDefinition(imposter);
            imposterDefinitionAction(imposterDefinition);

            _imposterHostBuilder.AddImposter(imposter);
            
            return _imposterHostBuilder;
        }
    }
}
