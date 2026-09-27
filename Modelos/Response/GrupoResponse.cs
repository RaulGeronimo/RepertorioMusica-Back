using System.Xml.Serialization;

namespace Modelos.Response
{
    public class GruposResponse
    {
        public int GrupoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Albumes { get; set; }
        public int Canciones { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public string Sellos { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public string SitioWeb { get; set; } = string.Empty;
        public string Idioma { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Grupos")]
    public class GruposListResponse<T>
    {
        [XmlElement("Grupo")]
        public List<T> Items { get; set; } = new();
    }
}
