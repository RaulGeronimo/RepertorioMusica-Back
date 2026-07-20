using System.Xml.Serialization;

namespace Modelos.Response
{
    public class GrupoBaseResponse
    {
        public int GrupoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Sellos { get; set; } = string.Empty;
        public string SitioWeb { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
    }

    public class GrupoResponse : GrupoBaseResponse
    {
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
        public int EstatusId { get; set; }
        public int IdiomaId { get; set; }
    }

    public class GruposResponse : GrupoBaseResponse
    {
        public int Albumes { get; set; }
        public int Canciones { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public string Idioma { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Grupos")]
    public class GruposListResponse<T>
    {
        [XmlElement("Grupo")]
        public List<T> Items { get; set; } = new();
    }
}
