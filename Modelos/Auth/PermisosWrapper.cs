using System.Xml.Serialization;

namespace Modelos.Auth
{
    [XmlRoot("Permisos")]
    public class PermisosWrapper
    {
        [XmlElement("PermisoResponse")]
        public required List<PermisoResponse> Items { get; set; }
    }
}
