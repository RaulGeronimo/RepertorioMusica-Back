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
    public class AlbumController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public AlbumController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] AlbumRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarAlbum", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Album, request.Nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{albumId}")]
        public async Task<IActionResult> Actualizar(int albumId, [FromBody] AlbumRequest request)
        {
            request.AlbumId = albumId;
            response = await _service.EjecutarSPConXml("scActualizarAlbum", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Album, request.Nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{albumId}")]
        public async Task<IActionResult> Eliminar(int albumId)
        {
            AlbumRequest request = new AlbumRequest { AlbumId = albumId };
            response = await _service.EjecutarSPConXml("scEliminarAlbum", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int id = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Album, nombre, ProcesoBitacora.Eliminado, id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, AlbumesListResponse<AlbumesResponse>, AlbumesResponse>("scObtenerAlbum");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{albumId}")]
        public async Task<IActionResult> ObtenerPorId(int albumId)
        {
            var (response, item) = await _service.EjecutarSPPorId<AlbumesListResponse<AlbumResponse>, AlbumResponse>("scBuscarAlbumId", "@AlbumId", albumId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
