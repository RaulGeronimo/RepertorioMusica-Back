using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Enums;
using Modelos.Request;
using Modelos.Response;
using Negocio;

namespace MusicaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GrupoController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public GrupoController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] GrupoRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Grupo, request.Nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{grupoId}")]
        public async Task<IActionResult> Actualizar(int grupoId, [FromBody] GrupoRequest request)
        {
            request.GrupoId = grupoId;
            response = await _service.EjecutarSPConXml("scActualizarGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Grupo, request.Nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{grupoId}")]
        public async Task<IActionResult> Eliminar(int grupoId)
        {
            GrupoRequest request = new GrupoRequest { GrupoId = grupoId };
            response = await _service.EjecutarSPConXml("scEliminarGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int id = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Grupo, nombre, ProcesoBitacora.Eliminado, id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, GruposListResponse<GruposResponse>, GruposResponse>("scObtenerGrupo");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{grupoId}")]
        public async Task<IActionResult> ObtenerPorId(int grupoId)
        {
            var (response, item) = await _service.EjecutarSPPorId<GruposListResponse<GrupoResponse>, GrupoResponse>("scBuscarGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
