using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public class ImpostersAsStubConfiguration
    {
        public RestImposter[] Imposters { get; }

        public ImpostersAsStubConfiguration(RestImposter[] imposters)
        {
            Imposters = imposters;
        }
    }
}
