using MusicaAPI.Extensions;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.Excel;
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
            var (response, lista) = await _service.EjecutarSPPorIdLista<AlbumesListResponse<AlbumesResponse>, AlbumesResponse>("scBuscarAlbumGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Grupo/Cancion/{grupoId}")]
        public async Task<IActionResult> ObtenerCancionesGrupo(int grupoId)
        {
            var (response, lista) = await _service.EjecutarSPPorIdLista<CancionesListResponse<CancionesResponse>, CancionesResponse>("scBuscarCancionGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Grupo/Integrante/{grupoId}")]
        public async Task<IActionResult> ObtenerIntegrantesGrupo(int grupoId)
        {
            var (response, lista) = await _service.EjecutarSPPorIdLista<ArtistasGrupoListResponse<ArtistasGrupoResponse>, ArtistasGrupoResponse>("scBuscarIntegranteGrupoId", "@GrupoId", grupoId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Grupo/Exportar/{grupoId}")]
        public async Task<IActionResult> ExportarGrupo(int grupoId)
        {
            var (responseAlbumes, albumes) = await _service.EjecutarSPPorIdLista<AlbumesListResponse<AlbumesResponse>, AlbumesResponse>("scBuscarAlbumGrupoId", "@GrupoId", grupoId);
            if (!responseAlbumes.Success)
                return BadRequest(responseAlbumes);

            var (responseCanciones, canciones) = await _service.EjecutarSPPorIdLista<CancionesListResponse<CancionesResponse>, CancionesResponse>("scBuscarCancionGrupoId", "@GrupoId", grupoId);
            if (!responseCanciones.Success)
                return BadRequest(responseCanciones);

            var (responseIntegrantes, integrantes) = await _service.EjecutarSPPorIdLista<ArtistasGrupoListResponse<ArtistasGrupoResponse>, ArtistasGrupoResponse>("scBuscarIntegranteGrupoId", "@GrupoId", grupoId);
            if (!responseIntegrantes.Success)
                return BadRequest(responseIntegrantes);

            var hayCanciones = albumes?.Any() == true;
            var hayAlbumes = canciones?.Any() == true;
            var hayIntegrantes = integrantes?.Any() == true;

            if (!hayCanciones && !hayAlbumes && !hayIntegrantes)
                return NotFound(new { mensaje = "No hay registros para exportar." });

            var bytes = ExcelExporter.GenerarMultiple(libro =>
            {
                if (hayCanciones)
                    libro.Hoja("Canciones", canciones!, new ExcelOptions
                    {
                        Excluir = { "Grupo" }
                    });

                if (hayAlbumes)
                    libro.Hoja("Albumes", albumes!, new ExcelOptions
                    {
                        Excluir = { "Grupo" }
                    });

                if (hayIntegrantes)
                    libro.Hoja("Integrantes", integrantes!, new ExcelOptions
                    {
                        Excluir = { "Grupo" }
                    });
            });

            return this.ExcelFile(bytes, "Reporte");
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
            var (response, lista) = await _service.EjecutarSPPorIdLista<CancionesAlbumListResponse<CancionesAlbumResponse>, CancionesAlbumResponse>("scBuscarPistaAlbumId", "@AlbumId", albumId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Album/Exportar/{albumId}")]
        public async Task<IActionResult> Exportar(int albumId)
        {
            var (response, lista) = await _service.EjecutarSPPorIdLista<CancionesAlbumListResponse<CancionesAlbumResponse>, CancionesAlbumResponse>("scBuscarPistaAlbumId", "@AlbumId", albumId);

            if (!response.Success)
                return BadRequest(response);

            if (lista == null || !lista.Any())
                return NotFound(new { mensaje = response.Message });

            var bytes = ExcelExporter.Generar(lista, new ExcelOptions
            {
                NombreHoja = "Canciones",
                Excluir = { "Album", "Interpretacion", "Grupo" }
            });

            return this.ExcelFile(bytes, "Reporte");
        }
        #endregion Album
    }
}
