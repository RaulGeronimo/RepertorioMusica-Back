namespace Modelos.Auth.Sesion
{
    public class RefreshTokenRevocarRequest
    {
        public string TokenHash { get; set; } = string.Empty;
    }
}
