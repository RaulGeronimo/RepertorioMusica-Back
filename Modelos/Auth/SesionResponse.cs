namespace Modelos.Auth
{
    public class SesionResponse
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool MantenerSesion { get; set; }
    }
}
