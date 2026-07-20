using System.Xml.Serialization;

namespace Modelos.Response
{
    public class BitacoraErrorResponse
    {
        public int BitacoraErrorId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Tabla { get; set; } = string.Empty;
        public string Columna { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int TotalRegistros { get; set; }
    }

    #region Lista Genérica
    [XmlRoot("Bitacora")]
    public class BitacoraErrorListResponse<T>
    {
        [XmlElement("Error")]
        public List<T> Items { get; set; } = new();
    }
    #endregion
}
