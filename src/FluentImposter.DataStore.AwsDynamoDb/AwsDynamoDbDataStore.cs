using System;

using FluentImposter.Core;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public class AwsDynamoDbDataStore: IDataStore
    {
        public Guid CreateSession()
        {
            throw new NotImplementedException();
        }

        public void EndSession(Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
