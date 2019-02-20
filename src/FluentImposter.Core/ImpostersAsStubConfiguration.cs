using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public class ImpostersAsStubConfiguration
    {
        public Imposter[] Imposters { get; }

        public ImpostersAsStubConfiguration(Imposter[] imposters)
        {
            Imposters = imposters;
        }
    }
}
