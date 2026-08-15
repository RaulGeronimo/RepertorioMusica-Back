using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Auth;
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

            // 5. Obtener permisos del usuario
            var permisosResponse = await _service.EjecutarSPConXml("scBuscarPermisos", usuario);
            if (permisosResponse.Success && permisosResponse.Result != null)
            {
                var wrapper = XmlHelper.DeserializeFromXml<PermisosWrapper>(permisosResponse.Result.OuterXml);
                usuario.Permisos = wrapper.Items;
            }

            // 6. Generar tokens
            var accessToken = _jwtHelper.GenerateToken(usuario);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            // 7. Devolver respuesta
            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiraEn = _jwtHelper.GetExpiration(),
                Permisos = usuario.Permisos
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(request.AccessToken);

            if (principal == null)
            { return Unauthorized("Token inválido"); }

            // 1. Extraer claims del token expirado
            var usuarioIdClaim = principal.FindFirst("UsuarioId");

            if (usuarioIdClaim == null)
            {
                return Unauthorized("Token inválido");
            }

            var usuario = new AuthResponse
            {
                UsuarioId = int.Parse(usuarioIdClaim.Value)
            };

            // 2. Consultar los permisos del usuario
            var permisosResponse = await _service.EjecutarSPConXml("scBuscarPermisos", usuario);
            if (permisosResponse.Success && permisosResponse.Result != null)
            {
                var wrapper = XmlHelper.DeserializeFromXml<PermisosWrapper>(permisosResponse.Result.OuterXml);
                usuario.Permisos = wrapper.Items;
            }

            // 3. Generar nuevos tokens
            var newAccessToken = _jwtHelper.GenerateToken(usuario);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // 4. Retornar los tokens con permisos
            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiraEn = _jwtHelper.GetExpiration(),
                Permisos = usuario.Permisos
            });
        }

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
