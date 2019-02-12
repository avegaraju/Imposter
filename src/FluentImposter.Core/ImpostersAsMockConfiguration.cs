using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public class ImpostersAsMockConfiguration
    {
        public Imposter[] Imposters { get;}
        internal IDataStore DataStore { get;} 

        public ImpostersAsMockConfiguration(Imposter[] imposters,
                                           IDataStore dataStore)
        {
            Imposters = imposters;
            DataStore = dataStore;
        }
    }
}
