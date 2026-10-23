namespace Modelos.Auth.QR
{
    public class QrConsumirRequest
    {
        public string CodigoHash { get; set; } = string.Empty;
        public string? Ip { get; set; }
        public string? Dispositivo { get; set; }
    }
}
