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
    public class CancionAlbumController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public CancionAlbumController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] CancionAlbumRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarCancionAlbum", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.CancionAlbum, nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{cancionAlbumId}")]
        public async Task<IActionResult> Actualizar(int cancionAlbumId, [FromBody] CancionAlbumRequest request)
        {
            request.CancionAlbumId = cancionAlbumId;
            response = await _service.EjecutarSPConXml("scActualizarCancionAlbum", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.CancionAlbum, nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{cancionAlbumId}")]
        public async Task<IActionResult> Eliminar(int cancionAlbumId)
        {
            CancionAlbumRequest request = new CancionAlbumRequest { CancionAlbumId = cancionAlbumId };
            response = await _service.EjecutarSPConXml("scEliminarCancionAlbum", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.CancionAlbum, nombre, ProcesoBitacora.Eliminado, registroId);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, ArtistasGrupoListResponse<ArtistasGrupoResponse>, ArtistasGrupoResponse>("scObtenerCancionAlbum");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{cancionAlbumId}")]
        public async Task<IActionResult> ObtenerPorId(int cancionAlbumId)
        {
            var (response, item) = await _service.EjecutarSPPorId<ArtistasGrupoListResponse<CancionAlbumResponse>, CancionAlbumResponse>("scBuscarCancionAlbumId", "@CancionAlbumId", cancionAlbumId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
