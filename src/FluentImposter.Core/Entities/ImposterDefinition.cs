using System.Net.Http;

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

        public ImposterRule Set(string resourcePath, HttpMethod httpMethod)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetMethod(httpMethod);

            return new ImposterRule(_imposter);
        }

        public ImposterRule StubsResource(string resourcePath, HttpMethod method)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetBehavior(ImposterBehavior.Stub);
            _imposter.SetMethod(method);

            return new ImposterRule(_imposter);
        }

        public ImposterRule MocksResource(string resourcePath, HttpMethod method)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetBehavior(ImposterBehavior.Mock);
            _imposter.SetMethod(method);

            return new ImposterRule(_imposter);
        }
    }
}
