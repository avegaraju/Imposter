using System;

namespace FluentImposter.Core
{
    public interface IDataStore
    {
        Guid CreateSession();
        void EndSession(Guid guid);
    }
}
