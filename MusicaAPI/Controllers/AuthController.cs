using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Auth;
using Modelos.Auth.QR;
using Modelos.Auth.Sesion;
using Modelos.Enums;
using Modelos.Request;
using Modelos.Response;
using Negocio;

namespace MusicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DynamicService _service;
        private readonly IJwtHelper _jwtHelper;
        GenericResponse response = new GenericResponse();

        public AuthController(DynamicService service, IJwtHelper jwtHelper)
        {
            _service = service;
            _jwtHelper = jwtHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] UsuarioRequest request)
        {
            request.Password = (request.Password != "") ? BCrypt.Net.BCrypt.HashPassword(request.Password) : "";

            response = await _service.EjecutarSPConXml("scGuardarUsuario", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Usuarios, nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{usuarioId}")]
        public async Task<IActionResult> Actualizar(int usuarioId, [FromBody] UsuarioRequest request)
        {
            request.UsuarioId = usuarioId;
            request.Password = (request.Password != "") ? BCrypt.Net.BCrypt.HashPassword(request.Password) : "";

            response = await _service.EjecutarSPConXml("scActualizarUsuario", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Usuarios, nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("usuario")]
        public async Task<IActionResult> ObtenerPorId()
        {
            int usuarioId = _service.ResolverUsuarioId();
            var (response, item) = await _service.EjecutarSPPorId<UsuariosListResponse<UsuariosResponse>, UsuariosResponse>("scBuscarUsuarioId", "@UsuarioId", usuarioId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(); }

            return Ok(item);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, UsuariosListResponse<UsuariosResponse>, UsuariosResponse>("scObtenerUsuario");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpPost("login")]
        public async Task<IActionResult> IniciarSesion([FromBody] LoginRequest request)
        {
            // 1. Obtener usuario desde SP
            response = await _service.EjecutarSPConXml("scIniciarSesion", request);

            if (!response.Success || response.Result == null)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return Unauthorized(response);
            }

            // 2. Deserializar el usuario
            var usuario = XmlHelper.DeserializeFromXml<AuthResponse>(response.Result.OuterXml);

            // 3. Validar si el usuario esta activo
            if (!usuario.Activo)
            {
                response.Success = false;
                response.Message = "Acceso denegado";
                response.Result = null;
                response.Code = "100_404";

                response.Message = await _service.RegistrarBitacoraError(response.Code, usuario.UsuarioId);
                return Unauthorized(response);
            }

            // 4. Verificar contraseña
            bool esValida = BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password);
            if (!esValida)
            {
                response.Success = false;
                response.Message = "Contraseña inválida";
                response.Result = null;
                response.Code = "100_403";

                response.Message = await _service.RegistrarBitacoraError(response.Code, usuario.UsuarioId);
                return Unauthorized(response);
            }

            // 5. Permisos + tokens (el refresh token se guarda hasheado en BD)
            usuario.Password = string.Empty;
            var tokens = await EmitirSesionNuevaAsync(usuario, request.MantenerSesion);
            if (tokens == null)
            { return StatusCode(500, Fallo("No se pudo iniciar la sesión.", "500")); }

            // 6. Devolver respuesta
            return Ok(tokens);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            { return Unauthorized(Fallo("La sesión no es válida.", "100_601")); }

            // 1. Validar y rotar el refresh token en BD (revoca el anterior y emite uno nuevo)
            var nuevoRefreshToken = _jwtHelper.GenerateRefreshToken();

            response = await _service.EjecutarSPConXml("scRotarRefreshToken", new RefreshTokenRotarRequest
            {
                TokenHashActual = _jwtHelper.HashToken(request.RefreshToken),
                TokenHashNuevo = _jwtHelper.HashToken(nuevoRefreshToken),
                DiasMantener = _jwtHelper.RefreshDias(true),
                DiasNormal = _jwtHelper.RefreshDias(false),
                GraciaSegundos = 30,
                Ip = ObtenerIp(),
                Dispositivo = ObtenerDispositivo()
            });

            if (!response.Success || response.Result == null)
            { return Unauthorized(response); }

            var sesion = XmlHelper.DeserializeFromXml<SesionResponse>(response.Result.OuterXml);

            // 2. Permisos actuales del usuario (por si cambiaron desde el último token)
            var usuario = new AuthResponse
            {
                UsuarioId = sesion.UsuarioId,
                Usuario = sesion.Usuario,
                Activo = sesion.Activo
            };
            await CargarPermisosAsync(usuario);

            // 3. Retornar tokens
            return Ok(new TokenResponse
            {
                AccessToken = _jwtHelper.GenerateToken(usuario),
                RefreshToken = nuevoRefreshToken,
                ExpiraEn = _jwtHelper.GetExpiration(),
                MantenerSesion = sesion.MantenerSesion,
                Permisos = usuario.Permisos
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                await _service.EjecutarSPConXml("scRevocarRefreshToken", new RefreshTokenRevocarRequest
                {
                    TokenHash = _jwtHelper.HashToken(request.RefreshToken)
                });
            }

            return Ok(new GenericResponse { Success = true, Message = "Sesión cerrada", Code = "200" });
        }

        #region Login con QR
        [Authorize]
        [HttpPost("qr/generar")]
        public async Task<IActionResult> GenerarQr()
        {
            var usuarioId = _service.UsuarioIdActual;
            if (usuarioId == null)
            { return Unauthorized(Fallo("La sesión no es válida.", "100_601")); }

            var codigo = _jwtHelper.GenerateSecureCode();
            var segundos = _jwtHelper.QrSegundos;

            response = await _service.EjecutarSPConXml("scGenerarQrLogin", new QrGenerarRequest
            {
                UsuarioId = usuarioId.Value,
                CodigoHash = _jwtHelper.HashToken(codigo),
                Segundos = segundos,
                Ip = ObtenerIp()
            });

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(new QrGenerarResponse { Contenido = PrefijoQr + codigo, Segundos = segundos });
        }

        [HttpPost("qr/login")]
        public async Task<IActionResult> LoginConQr([FromBody] QrLoginRequest request)
        {
            var contenido = request.Contenido?.Trim() ?? string.Empty;

            if (!contenido.StartsWith(PrefijoQr) || contenido.Length > 200)
            { return Unauthorized(Fallo("El código QR no es válido.", "100_604")); }

            var codigo = contenido.Substring(PrefijoQr.Length);

            response = await _service.EjecutarSPConXml("scConsumirQrLogin", new QrConsumirRequest
            {
                CodigoHash = _jwtHelper.HashToken(codigo),
                Ip = ObtenerIp(),
                Dispositivo = ObtenerDispositivo()
            });

            if (!response.Success || response.Result == null)
            { return Unauthorized(response); }

            var sesion = XmlHelper.DeserializeFromXml<SesionResponse>(response.Result.OuterXml);

            var usuario = new AuthResponse
            {
                UsuarioId = sesion.UsuarioId,
                Usuario = sesion.Usuario,
                Activo = sesion.Activo
            };

            var tokens = await EmitirSesionNuevaAsync(usuario, request.MantenerSesion);
            if (tokens == null)
            { return StatusCode(500, Fallo("No se pudo iniciar la sesión.", "500")); }

            return Ok(tokens);
        }
        #endregion Login con QR

        #region Helpers de sesión
        private const string PrefijoQr = "RPQR1:";

        private string? ObtenerIp() => HttpContext.Connection.RemoteIpAddress?.ToString();

        private string? ObtenerDispositivo()
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(userAgent)) return null;
            return userAgent.Length > 200 ? userAgent.Substring(0, 200) : userAgent;
        }

        private static GenericResponse Fallo(string mensaje, string code) =>
            new GenericResponse { Success = false, Message = mensaje, Code = code };

        private async Task CargarPermisosAsync(AuthResponse usuario)
        {
            var permisosResponse = await _service.EjecutarSPConXml("scBuscarPermisos", usuario);
            if (permisosResponse.Success && permisosResponse.Result != null)
            {
                var wrapper = XmlHelper.DeserializeFromXml<PermisosWrapper>(permisosResponse.Result.OuterXml);
                usuario.Permisos = wrapper.Items;
            }
        }

        private async Task<TokenResponse?> EmitirSesionNuevaAsync(AuthResponse usuario, bool mantenerSesion)
        {
            await CargarPermisosAsync(usuario);

            var refreshToken = _jwtHelper.GenerateRefreshToken();

            var guardado = await _service.EjecutarSPConXml("scGuardarRefreshToken", new RefreshTokenGuardarRequest
            {
                UsuarioId = usuario.UsuarioId,
                TokenHash = _jwtHelper.HashToken(refreshToken),
                FamiliaId = Guid.NewGuid(),
                MantenerSesion = mantenerSesion,
                DiasVigencia = _jwtHelper.RefreshDias(mantenerSesion),
                Ip = ObtenerIp(),
                Dispositivo = ObtenerDispositivo()
            });

            if (!guardado.Success)
            { return null; }

            return new TokenResponse
            {
                AccessToken = _jwtHelper.GenerateToken(usuario),
                RefreshToken = refreshToken,
                ExpiraEn = _jwtHelper.GetExpiration(),
                MantenerSesion = mantenerSesion,
                Permisos = usuario.Permisos
            };
        }
        #endregion Helpers de sesión

        [HttpPost("validar")]
        public async Task<IActionResult> Validar([FromBody] UsuarioRequest request)
        {
            var (response, usuarios) = await _service.EjecutarSPConXmlLista<UsuarioRequest, UsuariosListResponse<UsuariosResponse>, UsuariosResponse>("scValidarUsuario", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            if (response.Result == null)
            { return BadRequest(response); }

            return Ok(usuarios);
        }
    }
}