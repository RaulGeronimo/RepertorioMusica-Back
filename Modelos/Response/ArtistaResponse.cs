using System.Xml.Serialization;

namespace Modelos.Response
{
    public class ArtistasResponse
    {
        public int ArtistaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreArtistico { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public DateTime? FechaFinado { get; set; }
        public int Edad { get; set; }
        public decimal? Estatura { get; set; }
        public string Pais { get; set; } = string.Empty;
        public string Instrumentos { get; set; } = string.Empty;
        public string TipoVoz { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Artistas")]
    public class ArtistasListResponse<T>
    {
        [XmlElement("Artista")]
        public List<T> Items { get; set; } = new();
    }
}
