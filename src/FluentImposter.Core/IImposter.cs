using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IImposter
    {
        Imposter Build(ImposterDefinition imposterDefinition);
    }
}
