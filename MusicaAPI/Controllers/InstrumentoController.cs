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
    public class InstrumentoController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public InstrumentoController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] InstrumentoRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarInstrumento", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Instrumento, request.Nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{instrumentoId}")]
        public async Task<IActionResult> Actualizar(int instrumentoId, [FromBody] InstrumentoRequest request)
        {
            request.InstrumentoId = instrumentoId;
            response = await _service.EjecutarSPConXml("scActualizarInstrumento", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Instrumento, request.Nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{instrumentoId}")]
        public async Task<IActionResult> Eliminar(int instrumentoId)
        {
            InstrumentoRequest request = new InstrumentoRequest { InstrumentoId = instrumentoId };
            response = await _service.EjecutarSPConXml("scEliminarInstrumento", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int id = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Instrumento, nombre, ProcesoBitacora.Eliminado, id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, InstrumentosListResponse<InstrumentosResponse>, InstrumentosResponse>("scObtenerInstrumento");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{instrumentoId}")]
        public async Task<IActionResult> ObtenerPorId(int instrumentoId)
        {
            var (response, item) = await _service.EjecutarSPPorId<InstrumentosListResponse<InstrumentoResponse>, InstrumentoResponse>("scBuscarInstrumentoId", "@InstrumentoId", instrumentoId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
