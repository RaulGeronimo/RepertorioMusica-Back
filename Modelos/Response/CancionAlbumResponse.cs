using System.Xml.Serialization;

namespace Modelos.Response
{
    public class CancionAlbumBaseResponse
    {
        public int CancionAlbumId { get; set; }
        public int Numero { get; set; }
    }

    public class CancionAlbumResponse : CancionAlbumBaseResponse
    {
        public int AlbumId { get; set; }
        public int CancionId { get; set; }
    }

    public class CancionesAlbumResponse : CancionAlbumBaseResponse
    {
        public string Album { get; set; } = string.Empty;
        public string Cancion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public DateTime Publicacion { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string Interpretacion { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Canciones")]
    public class CancionesAlbumListResponse<T>
    {
        [XmlElement("Cancion")]
        public List<T> Items { get; set; } = new();
    }
}
