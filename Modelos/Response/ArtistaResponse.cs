using System.Xml.Serialization;

namespace Modelos.Response
{
    public class ArtistaBaseResponse
    {
        public int ArtistaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreArtistico { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public DateTime? FechaFinado { get; set; }
        public decimal? Estatura { get; set; }
        public string Instrumentos { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
    }

    public class ArtistaResponse : ArtistaBaseResponse
    {
        public int GeneroId { get; set; }
        public int PaisId { get; set; }
        public int TipoVozId { get; set; }
    }

    public class ArtistasResponse : ArtistaBaseResponse
    {
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Pais { get; set; } = string.Empty;
        public string TipoVoz { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Artistas")]
    public class ArtistasListResponse<T>
    {
        [XmlElement("Artista")]
        public List<T> Items { get; set; } = new();
    }
}
