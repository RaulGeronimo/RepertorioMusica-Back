using System.Xml.Serialization;

namespace Modelos.Auth
{
    public class AuthResponse
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public List<PermisoResponse> Permisos { get; set; } = new();
    }

    [XmlRoot("Usuarios")]
    public class AuthListResponse
    {
        [XmlElement("Usuario")]
        public List<AuthResponse> Items { get; set; } = new List<AuthResponse>();
    }
}
