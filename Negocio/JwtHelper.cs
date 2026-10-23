using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Modelos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Negocio
{
    public interface IJwtHelper
    {
        string GenerateToken(AuthResponse usuario);
        string GenerateToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        string GenerateSecureCode();
        string HashToken(string token);
        int RefreshDias(bool mantenerSesion);
        int QrSegundos { get; }
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        DateTime GetExpiration();
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;
        private readonly int _expiresInMinutes;
        private readonly int _refreshDiasMantener;
        private readonly int _refreshDiasNormal;

        public int QrSegundos { get; }

        public JwtHelper(IConfiguration config)
        {
            _config = config;

            _expiresInMinutes = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var minutes) ? minutes : 60;
            _refreshDiasMantener = int.TryParse(_config["Jwt:RefreshDiasMantener"], out var dm) ? dm : 30;
            _refreshDiasNormal = int.TryParse(_config["Jwt:RefreshDiasNormal"], out var dn) ? dn : 1;
            QrSegundos = int.TryParse(_config["Jwt:QrSegundos"], out var qs) ? qs : 60;
        }

        public DateTime GetExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_expiresInMinutes);
        }

        public int RefreshDias(bool mantenerSesion)
        {
            return mantenerSesion ? _refreshDiasMantener : _refreshDiasNormal;
        }

        private IEnumerable<Claim> BuildClaims(AuthResponse usuario)
        {
            var claims = new List<Claim>
            {
                new Claim("UsuarioId", usuario.UsuarioId.ToString()),
                new Claim("Usuario", usuario.Usuario)
            };

            foreach (var permiso in usuario.Permisos)
            {
                claims.Add(new Claim(
                    "Permisos",
                    $"{permiso.SeccionId}|{permiso.Seccion}|crear:{permiso.PuedeCrear},editar:{permiso.PuedeEditar},eliminar:{permiso.PuedeEliminar},ver:{permiso.PuedeVer}"
                ));
            }

            return claims;
        }

        public string GenerateToken(AuthResponse usuario)
        {
            return GenerateToken(BuildClaims(usuario));
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: GetExpiration(),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        // Código aleatorio de 256 bits en base64url (seguro para QR y URLs)
        public string GenerateSecureCode()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        }

        // SHA-256 en hexadecimal minúsculas. En BD solo se guarda el hash, nunca el token.
        public string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
    }
}
