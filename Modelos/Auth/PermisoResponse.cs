namespace Modelos.Auth
{
    public class PermisoResponse
    {
        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int SeccionId { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public bool PuedeVer { get; set; }
    }
}
