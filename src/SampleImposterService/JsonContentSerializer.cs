using System.IO;
using System.Text;

using FluentImposter.Core;

using Newtonsoft.Json;

namespace ImpostersServiceSample
{
    internal class JsonContentSerializer : IContentSerializer
    {
        public string Serialize(object content)
        {
            var jsonSerializer = new JsonSerializer();

            StringBuilder contentStringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(contentStringBuilder))
            {
                jsonSerializer.Serialize(stringWriter, content);
            }

            return contentStringBuilder.ToString();
        }
    }
}
