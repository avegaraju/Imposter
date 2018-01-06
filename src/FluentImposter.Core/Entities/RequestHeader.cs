using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FluentImposter.Core.Entities
{
    public class RequestHeader: IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        internal readonly Dictionary<string, IEnumerable<string>> HeadersInternal;

        public RequestHeader()
        {
            HeadersInternal = new Dictionary<string, IEnumerable<string>>();
        }

        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return HeadersInternal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key, string[] values)
        {
            HeadersInternal.Add(key, values);
        }

        public bool Contains(string key)
        {
            return HeadersInternal.Keys.Contains(key);
        }

        public bool ContainsKeyAndValues(string key, string[] values)
        {
            if (!HeadersInternal.ContainsKey(key))
            {
                return false;
            }

            HeadersInternal.TryGetValue(key, out IEnumerable<string> valuesOfKey);

            return !values.Except(valuesOfKey).Any();
        }
    }
}
