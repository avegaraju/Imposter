using System.Net.Http;
using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public class RestResource
    {
        private readonly RestImposter _imposter;

        public RestResource(RestImposter imposter)
        {
            _imposter = imposter;
        }

        public ImposterRule DeclareResource(string resourcePath, HttpMethod httpMethod)
        {
            _imposter.SetResource(resourcePath);
            _imposter.SetMethod(httpMethod);

            return new ImposterRule(_imposter);
        }
    }
}
