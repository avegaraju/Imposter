using System;

namespace FluentImposter.Core.Entities
{
    public class Request
    {
        public RequestHeader RequestHeader { get; set; } = new RequestHeader();
        public string Content { get; set; } = string.Empty;
        internal Guid SessionId { get; set; }
    }
}

