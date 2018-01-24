using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public class ImposterConfiguration
    {
        public Imposter[] Imposters { get; private set; }
        internal IDataStore DataStore { get; private set; } = new NullDataStore();

        public ImposterConfiguration(Imposter[] imposters)
        {
            Imposters = imposters;
        }

        internal void SetDataStore(IDataStore dataStore)
        {
            DataStore = dataStore;
        }
    }
}
