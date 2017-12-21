using System;

using Imposter.Core.Entities;

namespace Imposter.Core.DSL
{
    public class ImposterHostBuilder
    {
        private Host _host;

        public ImposterHost Create()
        {
            return new ImposterHost(_host);
        }

        public IImposterBuilder HostedOn(Uri uri)
        {
            _host = new Host(uri);

            return new ImposterBuilder(this);
        }
    }
}
