using System;

namespace FluentImposter.Core.Entities
{
    public class SmtpImposter
    {
        private readonly string _imposterName;
        
        public Uri SmtpServerUri { get; private set; }
        public uint Port { get; private set; }

        public SmtpImposter(string imposterName)
        {
            _imposterName = imposterName;
        }

        public SmtpImposter SetSmtpServerUri(Uri uri)
        {
            SmtpServerUri = uri;

            return this;
        }

        public SmtpImposter AtPort(uint port)
        {
            Port = port;

            return this;
        }
    }
}
