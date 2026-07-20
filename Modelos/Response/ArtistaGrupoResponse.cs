using System.Xml.Serialization;

namespace Modelos.Response
{
    public class ArtistaGrupoBaseResponse
    {
        public int ArtistaGrupoId { get; set; }
    }

    public class ArtistaGrupoResponse : ArtistaGrupoBaseResponse
    {
        public int ArtistaId { get; set; }
        public int GrupoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public class ArtistasGrupoResponse : ArtistaGrupoBaseResponse
    {
        public string Nombre { get; set; } = string.Empty;
        public string NombreArtistico { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public DateTime? FechaFinado { get; set; }
        public int Edad { get; set; }
        public decimal? Estatura { get; set; }
        public string Pais { get; set; } = string.Empty;
        public string Instrumentos { get; set; } = string.Empty;
        public string TipoVoz { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Artistas")]
    public class ArtistasGrupoListResponse<T>
    {
        [XmlElement("Artista")]
        public List<T> Items { get; set; } = new();
    }
}
