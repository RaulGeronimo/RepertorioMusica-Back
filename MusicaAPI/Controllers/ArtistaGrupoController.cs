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
    public class ArtistaGrupoController : Controller
    {
        private readonly DynamicService _service;
        GenericResponse response = new GenericResponse();

        public ArtistaGrupoController(DynamicService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] ArtistaGrupoRequest request)
        {
            response = await _service.EjecutarSPConXml("scGuardarArtistaGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.ArtistaGrupo, nombre, ProcesoBitacora.Agregado, registroId);

            return Ok(response);
        }

        [HttpPut("{artistaGrupoId}")]
        public async Task<IActionResult> Actualizar(int artistaGrupoId, [FromBody] ArtistaGrupoRequest request)
        {
            request.ArtistaGrupoId = artistaGrupoId;
            response = await _service.EjecutarSPConXml("scActualizarArtistaGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.ArtistaGrupo, nombre, ProcesoBitacora.Actualizado, registroId);

            return Ok(response);
        }

        [HttpDelete("{artistaGrupoId}")]
        public async Task<IActionResult> Eliminar(int artistaGrupoId)
        {
            ArtistaGrupoRequest request = new ArtistaGrupoRequest { ArtistaGrupoId = artistaGrupoId };
            response = await _service.EjecutarSPConXml("scEliminarArtistaGrupo", request);

            if (!response.Success)
            {
                response.Message = await _service.RegistrarBitacoraError(response.Code);
                return BadRequest(response);
            }

            int registroId = int.TryParse(response.Result?["Id"]?.InnerText, out var parsedId) ? parsedId : 0;
            string nombre = response.Result?["Nombre"]?.InnerText!;
            response.Result = null;

            // Registrar bitácora
            await _service.RegistrarBitacoraCarga(SeccionBitacora.ArtistaGrupo, nombre, ProcesoBitacora.Eliminado, registroId);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, ArtistasGrupoListResponse<ArtistasGrupoResponse>, ArtistasGrupoResponse>("scObtenerArtistaGrupo");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("{artistaGrupoId}")]
        public async Task<IActionResult> ObtenerPorId(int artistaGrupoId)
        {
            var (response, item) = await _service.EjecutarSPPorId<ArtistasGrupoListResponse<ArtistaGrupoResponse>, ArtistaGrupoResponse>("scBuscarArtistaGrupoId", "@ArtistaGrupoId", artistaGrupoId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }
    }
}
