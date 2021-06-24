using System.Diagnostics;
using System.Net.Http;

using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public class ImposterDefinition
    {
        private readonly string _imposterName;
        private RestImposter _restImposter;

        public ImposterDefinition(string imposterName)
        {
            _imposterName = imposterName;
        }

        public ImposterRule DeclareResource(string resourcePath, HttpMethod httpMethod)
        {
            _restImposter.SetResource(resourcePath);
            _restImposter.SetMethod(httpMethod);

            return new ImposterRule(_restImposter);
        }

        public IImposter ForRest()
        {
            return new RestImposter(_imposterName);
        }

        public IImposter ForSmtp()
        {
            return new SmtpImposter();
        }
    }
}
