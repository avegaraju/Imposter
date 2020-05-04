using System;
using System.Net.Http;

namespace FluentImposter.Core.Entities
{
    public class Request
    {
        public RequestHeader RequestHeader { get; set; } = new RequestHeader();
        public string Content { get; set; } = string.Empty;
        public HttpRequestMessage Req { get; set; }
        internal Guid SessionId { get; set; }
    }
}

