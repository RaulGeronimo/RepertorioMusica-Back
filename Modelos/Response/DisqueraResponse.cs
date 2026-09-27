using System.Xml.Serialization;

namespace Modelos.Response
{
    public class DisquerasResponse
    {
        public int DisqueraId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fundacion { get; set; }
        public string Fundador { get; set; } = string.Empty;
        public string Generos { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Disqueras")]
    public class DisquerasListResponse<T>
    {
        [XmlElement("Disquera")]
        public List<T> Items { get; set; } = new();
    }
}
