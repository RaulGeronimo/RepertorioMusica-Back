namespace Modelos.Auth.Sesion
{
    public class RefreshTokenGuardarRequest
    {
        public int UsuarioId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public Guid FamiliaId { get; set; }
        public bool MantenerSesion { get; set; }
        public int DiasVigencia { get; set; }
        public string? Ip { get; set; }
        public string? Dispositivo { get; set; }
    }
}
