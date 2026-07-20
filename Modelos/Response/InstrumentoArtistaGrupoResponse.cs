using System.Xml.Serialization;

namespace Modelos.Response
{
    public class InstrumentoArtistaGrupoBaseResponse
    {
        public int InstrumentoArtistaGrupoId { get; set; }
    }

    public class InstrumentoArtistaGrupoResponse : InstrumentoArtistaGrupoBaseResponse
    {
        public int ArtistaId { get; set; }
        public int InstrumentoId { get; set; }
    }

    public class InstrumentosArtistaGrupoResponse : InstrumentoArtistaGrupoBaseResponse
    {
        public string Nombre { get; set; } = string.Empty;
        public string NombreArtistico { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string Instrumento { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Instrumentos")]
    public class InstrumentosArtistaGrupoListResponse<T>
    {
        [XmlElement("Instrumento")]
        public List<T> Items { get; set; } = new();
    }
}
