using System;

namespace Imposter.Core.Entities
{
    public class Host
    {
        public Uri BaseUri { get; }

        public Host(Uri baseUri)
        {
            BaseUri = baseUri;
        }
    }
}
