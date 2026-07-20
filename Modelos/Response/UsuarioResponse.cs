using System.Xml.Serialization;

namespace Modelos.Response
{
    #region Base
    public class UsuarioBaseResponse
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime? Registro { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool Activo { get; set; }
        public int RolId { get; set; }
    }
    #endregion

    #region Buscar (Listado)
    public class UsuariosResponse : UsuarioBaseResponse
    {
        public string Rol { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string DiasCumple { get; set; } = string.Empty;
        public int TotalRegistros { get; set; }
    }
    #endregion

    #region Obtener (Detalle)
    public class UsuarioResponse : UsuarioBaseResponse
    {
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    #endregion

    #region Lista Genérica
    [XmlRoot("Usuarios")]
    public class UsuariosListResponse<T>
    {
        [XmlElement("Usuario")]
        public List<T> Items { get; set; } = new();
    }
    #endregion
}