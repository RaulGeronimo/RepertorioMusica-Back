using System.Xml;
using System.Xml.Serialization;

namespace Modelos.Response
{
    [XmlRoot("Response")]
    public class GenericResponse
    {
        [XmlElement("Success")]
        public bool Success { get; set; }

        [XmlElement("Message")]
        public string Message { get; set; } = string.Empty;

        [XmlElement("Result")]
        public XmlElement? Result { get; set; }

        [XmlElement("Code")]
        public string Code { get; set; } = string.Empty;
    }
}
