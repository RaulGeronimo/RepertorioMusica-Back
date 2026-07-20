using System.Xml.Serialization;

namespace Modelos.Response
{
    public class AlbumBaseResponse
    {
        public int AlbumId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public DateTime Lanzamiento { get; set; }
        public string Grabacion { get; set; } = string.Empty;
        public string Portada { get; set; } = string.Empty;
    }

    public class AlbumResponse : AlbumBaseResponse
    {
        public int GrupoId { get; set; }
        public int DisqueraId { get; set; }
    }

    public class AlbumesResponse : AlbumBaseResponse
    {
        public string Grupo { get; set; } = string.Empty;
        public string Disquera { get; set; } = string.Empty;
        public int Canciones { get; set; }
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Albumes")]
    public class AlbumesListResponse<T>
    {
        [XmlElement("Album")]
        public List<T> Items { get; set; } = new();
    }
}
