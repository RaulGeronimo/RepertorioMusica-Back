using System.Xml.Serialization;

namespace Modelos.Response
{
    public class CancionBaseResponse
    {
        public int CancionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public DateTime Publicacion { get; set; }
        public string Genero { get; set; } = string.Empty;
    }

    public class CancionResponse : CancionBaseResponse
    {
        public int InterpretacionId { get; set; }
        public int GrupoId { get; set; }
    }

    public class CancionesResponse : CancionBaseResponse
    {
        public string Interpretacion { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Canciones")]
    public class CancionesListResponse<T>
    {
        [XmlElement("Cancion")]
        public List<T> Items { get; set; } = new();
    }
}
