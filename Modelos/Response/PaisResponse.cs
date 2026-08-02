using System.Xml.Serialization;

namespace Modelos.Response
{
    public class PaisBaseResponse
    {
        public int PaisId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public string Bandera { get; set; } = string.Empty;
    }

    public class PaisResponse : PaisBaseResponse { 
        public int ContinenteId { get; set; }
    }

    public class PaisesResponse : PaisBaseResponse
    {
        public string Continente { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Paises")]
    public class PaisesListResponse<T>
    {
        [XmlElement("Pais")]
        public List<T> Items { get; set; } = new();
    }
}