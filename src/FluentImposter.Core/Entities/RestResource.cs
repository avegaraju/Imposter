using System.Net.Http;
using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public class RestResource
    {
        private readonly RestImposter _restImposter;

        public RestResource(string imposterName)
        {
            _restImposter = new RestImposter(imposterName);
        }

        public ImposterRule DeclareResource(string resourcePath, HttpMethod httpMethod)
        {
            _restImposter.SetResource(resourcePath);
            _restImposter.SetMethod(httpMethod);

            return new ImposterRule(_restImposter);
        }
    }
}
