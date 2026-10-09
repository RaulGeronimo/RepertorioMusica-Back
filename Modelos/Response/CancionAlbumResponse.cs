using System.Xml.Serialization;

namespace Modelos.Response
{
    public class CancionesAlbumResponse
    {
        public int CancionAlbumId { get; set; }
        public string Album { get; set; } = string.Empty;
        public string Cancion { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Duracion { get; set; } = string.Empty;
        public DateTime Publicacion { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string Interpretacion { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string SpotifyId { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Canciones")]
    public class CancionesAlbumListResponse<T>
    {
        [XmlElement("Cancion")]
        public List<T> Items { get; set; } = new();
    }
}
