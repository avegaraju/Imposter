using FluentImposter.Core.DSL;
using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IImposter: IResponseCreator
    {
        Imposter Declare(ImposterDefinition imposterDefinirion);
    }
}
