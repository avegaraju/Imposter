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

        public RestResource ForRest()
        {
            var imposter =  new RestImposter(_imposterName);
         
            return new RestResource(imposter);
        }

        public SmtpImposter ForSmtp()
        {
            return new SmtpImposter();
        }
    }
}
