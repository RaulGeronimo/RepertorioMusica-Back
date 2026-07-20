using System.Xml.Serialization;

namespace Modelos.Response
{
    public class ErrorResponse
    {
        public int ErrorId { get; set; }
        public string CodigoGenerico { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string MensajeGeneral { get; set; } = string.Empty;
        public string Columna { get; set; } = string.Empty;
        public string MensajeError { get; set; } = string.Empty;
        public int SeccionId { get; set; }
        public int TotalRegistros { get; set; }
    }

    #region Lista Genérica
    [XmlRoot("Errores")]
    public class ErroresListResponse<T>
    {
        [XmlElement("Error")]
        public List<T> Items { get; set; } = new();
    }
    #endregion
}
