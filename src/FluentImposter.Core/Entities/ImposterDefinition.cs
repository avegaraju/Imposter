using System.Diagnostics;
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

        public ImposterRule DeclareResource(string resourcePath, HttpMethod httpMethod)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetMethod(httpMethod);

            return new ImposterRule(_imposter);
        }

        public IImposter OfType(ImposterOfType type)
        {
            IImposter imposter = null;
            switch(type)
            {
                case ImposterOfType.Rest:
                    imposter = new RestImposter();
                    break;
                case ImposterOfType.Smtp:
                    imposter = new SmtpImposter();
                    break;
            }

            return imposter;
        }
    }
}
