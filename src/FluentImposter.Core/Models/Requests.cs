using System;

namespace FluentImposter.Core.Models
{
    public class Requests
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string Resource { get; set; }
        public string HttpMethod { get; set; }
        public string RequestPayloadBase64 { get; set; }
    }
}
