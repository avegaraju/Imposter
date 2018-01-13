using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace FluentImposter.Core.Tests.Unit.Helpers
{
    internal class XmlContentSerializer: IContentSerializer
    {
        public string Serialize(object content)
        {
            XmlSerializer serializer = new XmlSerializer(content.GetType());

            StringBuilder contentStringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(contentStringBuilder))
            {
                serializer.Serialize(stringWriter, content);
            }

            return contentStringBuilder.ToString();
        }
    }
}
