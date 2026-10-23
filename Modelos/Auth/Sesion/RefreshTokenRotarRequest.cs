namespace Modelos.Auth.Sesion
{
    public class RefreshTokenRotarRequest
    {
        public string TokenHashActual { get; set; } = string.Empty;
        public string TokenHashNuevo { get; set; } = string.Empty;
        public int DiasMantener { get; set; }
        public int DiasNormal { get; set; }
        public int GraciaSegundos { get; set; }
        public string? Ip { get; set; }
        public string? Dispositivo { get; set; }
    }
}
