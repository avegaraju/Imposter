using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterStub
    {
        private readonly Imposter _imposter;

        public ImposterStub(Imposter imposter)
        {
            _imposter = imposter;
        }

        public ImposterRule StubsResource(string resourcePath)
        {
            _imposter.SetResource(resourcePath);

            return new ImposterRule(_imposter);
        }
    }
}
