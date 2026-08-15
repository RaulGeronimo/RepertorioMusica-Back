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
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        DateTime GetExpiration();
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;
        private readonly int _expiresInMinutes;

        public JwtHelper(IConfiguration config)
        {
            _config = config;

            _expiresInMinutes = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var minutes) ? minutes : 60;
        }

        public DateTime GetExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_expiresInMinutes);
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
