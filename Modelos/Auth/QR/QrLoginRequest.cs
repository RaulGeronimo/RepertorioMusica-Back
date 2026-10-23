namespace Modelos.Auth.QR
{
    public class QrLoginRequest
    {
        public string Contenido { get; set; } = string.Empty;
        public bool MantenerSesion { get; set; }
    }
}
