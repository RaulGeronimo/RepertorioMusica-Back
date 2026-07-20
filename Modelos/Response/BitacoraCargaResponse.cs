using System.Xml.Serialization;

namespace Modelos.Response
{
    public class BitacoraCargaResponse
    {
        public int BitacoraCargaId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Tabla { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Proceso { get; set; } = string.Empty;
        public DateTime Modificado { get; set; }
        public int RegistroId { get; set; }
        public int TotalRegistros { get; set; }
    }

    #region Lista Genérica
    [XmlRoot("Bitacora")]
    public class BitacoraCargaListResponse<T>
    {
        [XmlElement("Carga")]
        public List<T> Items { get; set; } = new();
    }
    #endregion
}
