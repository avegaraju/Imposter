using System;

namespace FluentImposter.Core.Entities
{
    public class SmtpServer
    {
        private readonly SmtpImposter _smtpImposter;
        public SmtpServer(string imposterName)
        {
            _smtpImposter = new SmtpImposter(imposterName);
        }

        public SmtpImposter CreateServer(Uri uri)
        {
            return _smtpImposter.SetSmtpServerUri(uri);
        }
    }
}
