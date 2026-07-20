namespace Modelos.Request
{
    public class UsuarioRequest
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime? Registro { get; set; } = DateTime.Now;
        public DateTime? FechaNacimiento { get; set; }
        public int RolId { get; set; } = 0;
        public bool Activo { get; set; } = true;
    }
}
