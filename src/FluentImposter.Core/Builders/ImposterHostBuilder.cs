using System;
using System.Collections.Generic;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterHostBuilder
    {
        private Host _host;
        private readonly List<Imposter> _imposters = new List<Imposter>();

        public IReadOnlyCollection<Imposter> Imposters => _imposters;

        public ImposterHost Create()
        {
            return new ImposterHost(_host);
        }

        public IImposterBuilder HostedOn(Uri uri)
        {
            _host = new Host(uri);

            return new ImposterBuilder(this);
        }

        internal void AddImposter(Imposter imposter)
        {
            _imposters.Add(imposter);
        }
    }
}
