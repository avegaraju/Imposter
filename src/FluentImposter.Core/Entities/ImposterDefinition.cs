using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public class ImposterDefinition
    {
        private readonly Imposter _imposter;

        public ImposterDefinition(string imposterName)
        {
            _imposter = new Imposter(imposterName);
        }

        public ImposterRule StubsResource(string resourcePath)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetBehavior(ImposterBehavior.Stub);

            return new ImposterRule(_imposter);
        }

        public ImposterRule MocksResource(string resourcePath)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetBehavior(ImposterBehavior.Mock);

            return new ImposterRule(_imposter);
        }
    }
}
