using FluentImposter.AspnetCore.Tests.Integration.Spies;
using FluentImposter.Core;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal static class FakeDataStoreExtension
    {
        public static ImpostersAsMockConfiguration UseSpyDataStore(this ImpostersAsMockConfiguration impostersConfiguration, IDataStore spyDataStore)
        {
            impostersConfiguration.SetDataStore(spyDataStore);

            return impostersConfiguration;
        }
    }
}