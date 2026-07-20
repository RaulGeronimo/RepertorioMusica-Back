namespace Modelos.Auth
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiraEn { get; set; }
        public List<PermisoResponse> Permisos { get; set; } = new();
    }
}
