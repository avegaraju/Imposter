using System;
using System.Threading.Tasks;

namespace FluentImposter.Core
{
    public interface IDataStore
    {
        Guid CreateSession();
        void EndSession(Guid guid);
    }
}
