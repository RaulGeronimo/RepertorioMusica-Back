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
    public class DisqueraController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public DisqueraController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] DisqueraRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarDisquera", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Disquera, request.Nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{disqueraId}")]
        public async Task<IActionResult> Actualizar(int disqueraId, [FromBody] DisqueraRequest request)
        {
            request.DisqueraId = disqueraId;
            response = await _service.EjecutarSPConXml("scActualizarDisquera", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Disquera, request.Nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{disqueraId}")]
        public async Task<IActionResult> Eliminar(int disqueraId)
        {
            DisqueraRequest request = new DisqueraRequest { DisqueraId = disqueraId };
            response = await _service.EjecutarSPConXml("scEliminarDisquera", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int id = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Disquera, nombre, ProcesoBitacora.Eliminado, id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, DisquerasListResponse<DisquerasResponse>, DisquerasResponse>("scObtenerDisquera");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{disqueraId}")]
        public async Task<IActionResult> ObtenerPorId(int disqueraId)
        {
            var (response, item) = await _service.EjecutarSPPorId<DisquerasListResponse<DisqueraResponse>, DisqueraResponse>("scBuscarDisqueraId", "@DisqueraId", disqueraId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
