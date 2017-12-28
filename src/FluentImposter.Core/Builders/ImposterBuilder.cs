using System;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public interface IImposterBuilder
    {
        ImposterHostBuilder HasAnImposter(string name,
                                          Func<ImposterDefinition, Imposter> imposterDefinitionFunction);
    }

    public class ImposterBuilder: IImposterBuilder
    {
        private readonly ImposterHostBuilder _imposterHostBuilder;

        public ImposterBuilder(ImposterHostBuilder imposterHostBuilder)
        {
            _imposterHostBuilder = imposterHostBuilder;
        }

        public ImposterHostBuilder HasAnImposter(string name,
                                                 Func<ImposterDefinition,Imposter> imposterDefinitionFunction)
        {
            var imposterName = string.IsNullOrEmpty(name)
                                   ? Guid.NewGuid().ToString()
                                   : name;

            var imposterDefinition = new ImposterDefinition(imposterName);
            var imposter = imposterDefinitionFunction(imposterDefinition);

            _imposterHostBuilder.AddImposter(imposter);

            return _imposterHostBuilder;
        }
    }
}
