using System.Net.Http;

namespace FluentImposter.Core.Entities
{
    public class VerificationRequest
    {
        public string Resource { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string RequestPayload { get; set; }
    }
}
