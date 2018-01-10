using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterMock
    {
        private readonly Imposter _imposter;

        public ImposterMock(Imposter imposter)
        {
            _imposter = imposter;
        }

        public ImposterRule MocksResource(string resourcePath)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetBehavior(ImposterBehavior.Mock);

            return new ImposterRule(_imposter);
        }
    }
}
