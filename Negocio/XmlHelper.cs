using System.Xml;
using System.Xml.Serialization;

namespace Negocio
{
    public static class XmlHelper
    {
        public static string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = false,
                NewLineHandling = NewLineHandling.None
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);

            serializer.Serialize(xmlWriter, obj);
            return stringWriter.ToString();
        }

        public static T DeserializeFromXml<T>(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var stringReader = new StringReader(xml);
            return (T)xmlSerializer.Deserialize(stringReader);
        }
    }
}
