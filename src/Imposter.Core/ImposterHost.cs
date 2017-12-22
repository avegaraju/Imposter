using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IImposterHost
    {
        void Start();
    }

    public class ImposterHost: IImposterHost
    {
        public Host Host { get; }

        internal ImposterHost(Host host)
        {
            Host = host;
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}
