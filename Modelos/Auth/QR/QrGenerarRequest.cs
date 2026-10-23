namespace Modelos.Auth.QR
{
    public class QrGenerarRequest
    {
        public int UsuarioId { get; set; }
        public string CodigoHash { get; set; } = string.Empty;
        public int Segundos { get; set; }
        public string? Ip { get; set; }
    }
}
