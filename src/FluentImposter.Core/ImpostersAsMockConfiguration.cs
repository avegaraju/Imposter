using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public class ImpostersAsMockConfiguration
    {
        public RestImposter[] Imposters { get;}
        internal IDataStore DataStore { get;} 

        public ImpostersAsMockConfiguration(RestImposter[] imposters,
                                           IDataStore dataStore)
        {
            Imposters = imposters;
            DataStore = dataStore;
        }
    }
}
