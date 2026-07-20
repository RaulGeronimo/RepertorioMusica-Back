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
    public class ArtistaController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public ArtistaController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] ArtistaRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarArtista", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Artista, request.NombreArtistico, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{artistaId}")]
        public async Task<IActionResult> Actualizar(int artistaId, [FromBody] ArtistaRequest request)
        {
            request.ArtistaId = artistaId;
            response = await _service.EjecutarSPConXml("scActualizarArtista", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?.InnerText, out var id) ? id : 0;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Artista, request.NombreArtistico, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{artistaId}")]
        public async Task<IActionResult> Eliminar(int artistaId)
        {
            ArtistaRequest request = new ArtistaRequest { ArtistaId = artistaId };
            response = await _service.EjecutarSPConXml("scEliminarArtista", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int id = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.Artista, nombre, ProcesoBitacora.Eliminado, id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, ArtistasListResponse<ArtistasResponse>, ArtistasResponse>("scObtenerArtista");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{artistaId}")]
        public async Task<IActionResult> ObtenerPorId(int artistaId)
        {
            var (response, item) = await _service.EjecutarSPPorId<ArtistasListResponse<ArtistaResponse>, ArtistaResponse>("scBuscarArtistaId", "@ArtistaId", artistaId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
