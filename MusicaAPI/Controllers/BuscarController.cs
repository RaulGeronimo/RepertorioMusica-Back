using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Response;
using Negocio;

namespace MusicaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BuscarController : Controller
    {
        private readonly DynamicService _service;

        public BuscarController(DynamicService service)
        {
            _service = service;
        }

        #region Grupo
        [HttpGet("Grupo/{grupoId}")]
        public async Task<IActionResult> BuscarGrupo(int grupoId)
        {
            var (response, item) = await _service.EjecutarSPPorId<GruposListResponse<GruposResponse>, GruposResponse>("scObtenerGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }

        [HttpGet("Grupo/Album/{grupoId}")]
        public async Task<IActionResult> ObtenerAlbumesGrupo(int grupoId)
        {
            var (response, item) = await _service.EjecutarSPPorIdLista<AlbumesListResponse<AlbumesResponse>, AlbumesResponse>("scBuscarAlbumGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(item);
        }

        [HttpGet("Grupo/Cancion/{grupoId}")]
        public async Task<IActionResult> ObtenerCancionesGrupo(int grupoId)
        {
            var (response, item) = await _service.EjecutarSPPorIdLista<CancionesListResponse<CancionesResponse>, CancionesResponse>("scBuscarCancionGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(item);
        }

        [HttpGet("Grupo/Integrante/{grupoId}")]
        public async Task<IActionResult> ObtenerIntegrantesGrupo(int grupoId)
        {
            var (response, item) = await _service.EjecutarSPPorIdLista<ArtistasGrupoListResponse<ArtistasGrupoResponse>, ArtistasGrupoResponse>("scBuscarIntegranteGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(item);
        }
        #endregion Grupo

        #region Album
        [HttpGet("Album/{albumId}")]
        public async Task<IActionResult> BuscarAlbum(int albumId)
        {
            var (response, item) = await _service.EjecutarSPPorId<AlbumesListResponse<AlbumesResponse>, AlbumesResponse>("scObtenerAlbumId", "@AlbumId", albumId);

            if (!response.Success)
            { return BadRequest(response); }

            if (item == null)
            { return NotFound(response); }

            return Ok(item);
        }

        [HttpGet("Album/Cancion/{albumId}")]
        public async Task<IActionResult> ObtenerCancionesAlbum(int albumId)
        {
            var (response, item) = await _service.EjecutarSPPorIdLista<CancionesAlbumListResponse<CancionesAlbumResponse>, CancionesAlbumResponse>("scBuscarPistaAlbumId", "@AlbumId", albumId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(item);
        }
        #endregion Album
    }
}
