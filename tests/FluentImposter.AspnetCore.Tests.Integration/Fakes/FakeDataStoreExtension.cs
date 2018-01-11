using FluentImposter.AspnetCore.Tests.Integration.Spies;
using FluentImposter.Core;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal static class FakeDataStoreExtension
    {
        public static ImposterConfiguration UseSpyDataStore(this ImposterConfiguration imposterConfiguration, IDataStore spyDataStore)
        {
            imposterConfiguration.SetDataStore(spyDataStore);

            return imposterConfiguration;
        }
    }
}