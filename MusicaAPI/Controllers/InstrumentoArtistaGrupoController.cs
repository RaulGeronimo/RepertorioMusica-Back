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
    public class InstrumentoArtistaGrupoController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public InstrumentoArtistaGrupoController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] InstrumentoArtistaGrupoRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarInstrumentoArtistaGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.InstrumentoArtistaGrupo, nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{instrumentoArtistaGrupoId}")]
        public async Task<IActionResult> Actualizar(int instrumentoArtistaGrupoId, [FromBody] InstrumentoArtistaGrupoRequest request)
        {
            request.InstrumentoArtistaGrupoId = instrumentoArtistaGrupoId;
            response = await _service.EjecutarSPConXml("scActualizarInstrumentoArtistaGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.InstrumentoArtistaGrupo, nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{instrumentoArtistaGrupoId}")]
        public async Task<IActionResult> Eliminar(int instrumentoArtistaGrupoId)
        {
            InstrumentoArtistaGrupoRequest request = new InstrumentoArtistaGrupoRequest { InstrumentoArtistaGrupoId = instrumentoArtistaGrupoId };
            response = await _service.EjecutarSPConXml("scEliminarInstrumentoArtistaGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.InstrumentoArtistaGrupo, nombre, ProcesoBitacora.Eliminado, registroId);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, InstrumentosArtistaGrupoListResponse<InstrumentosArtistaGrupoResponse>, InstrumentosArtistaGrupoResponse>("scObtenerInstrumentoArtistaGrupo");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{instrumentoArtistaGrupoId}")]
        public async Task<IActionResult> ObtenerPorId(int instrumentoArtistaGrupoId)
        {
            var (response, item) = await _service.EjecutarSPPorId<InstrumentosArtistaGrupoListResponse<InstrumentoArtistaGrupoResponse>, InstrumentoArtistaGrupoResponse>("scBuscarInstrumentoArtistaGrupoId", "@InstrumentoArtistaGrupoId", instrumentoArtistaGrupoId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
