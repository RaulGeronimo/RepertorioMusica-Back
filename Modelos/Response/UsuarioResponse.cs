using System.Xml.Serialization;

namespace Modelos.Response
{
    public class UsuariosResponse
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime? Registro { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string DiasCumple { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int TotalRegistros { get; set; }
    }

    [XmlRoot("Usuarios")]
    public class UsuariosListResponse<T>
    {
        [XmlElement("Usuario")]
        public List<T> Items { get; set; } = new();
    }
}