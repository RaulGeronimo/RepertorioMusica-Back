using System.Xml.Serialization;

namespace Modelos.Response
{
    public class InstrumentoBaseResponse
    {
        public int InstrumentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
    }

    public class InstrumentoResponse : InstrumentoBaseResponse { }

    public class InstrumentosResponse : InstrumentoBaseResponse
    {
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Instrumentos")]
    public class InstrumentosListResponse<T>
    {
        [XmlElement("Instrumento")]
        public List<T> Items { get; set; } = new();
    }
}
