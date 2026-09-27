using System.Xml.Serialization;

namespace Modelos.Response
{
    public class AlbumesResponse
    {
        public int AlbumId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string Disquera { get; set; } = string.Empty;
        public int Canciones { get; set; }
        public string Duracion { get; set; } = string.Empty;
        public DateTime Lanzamiento { get; set; }
        public string Grabacion { get; set; } = string.Empty;
        public string Portada { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Albumes")]
    public class AlbumesListResponse<T>
    {
        [XmlElement("Album")]
        public List<T> Items { get; set; } = new();
    }
}
