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
    public class CancionController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public CancionController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] CancionRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarCancion", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Cancion, request.Nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{cancionId}")]
        public async Task<IActionResult> Actualizar(int cancionId, [FromBody] CancionRequest request)
        {
            request.CancionId = cancionId;
            response = await _service.EjecutarSPConXml("scActualizarCancion", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Cancion, request.Nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{cancionId}")]
        public async Task<IActionResult> Eliminar(int cancionId)
        {
            CancionRequest request = new CancionRequest { CancionId = cancionId };
            response = await _service.EjecutarSPConXml("scEliminarCancion", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int id = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Cancion, nombre, ProcesoBitacora.Eliminado, id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, CancionesListResponse<CancionesResponse>, CancionesResponse>("scObtenerCancion");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{cancionId}")]
        public async Task<IActionResult> ObtenerPorId(int cancionId)
        {
            var (response, item) = await _service.EjecutarSPPorId<CancionesListResponse<CancionResponse>, CancionResponse>("scBuscarCancionId", "@CancionId", cancionId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
