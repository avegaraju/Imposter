using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IImposter: IResponseCreator
    {
        Imposter Build(ImposterDefinition imposterDefinirion);
    }
}
