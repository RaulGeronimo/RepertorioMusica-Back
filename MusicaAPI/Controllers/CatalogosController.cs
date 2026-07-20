using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio;

namespace MusicaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogosController : Controller
    {
        private readonly DynamicService _service;
        public CatalogosController(DynamicService service)
        {
            _service = service;
        }

        [HttpGet("Rol")]
        public async Task<IActionResult> ObtenerRol()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("Rol");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }

        [HttpGet("Estatus")]
        public async Task<IActionResult> ObtenerEstatus()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("Estatus");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }

        [HttpGet("Idioma")]
        public async Task<IActionResult> ObtenerIdioma()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("Idioma");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }

        [HttpGet("Genero")]
        public async Task<IActionResult> ObtenerGenero()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("Genero");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }

        [HttpGet("Continente")]
        public async Task<IActionResult> ObtenerContinente()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("Continente");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }

        [HttpGet("Interpretacion")]
        public async Task<IActionResult> ObtenerInterpretacion()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("Interpretacion");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }

        [HttpGet("TipoVoz")]
        public async Task<IActionResult> ObtenerTipoVoz()
        {
            var (response, resultados) = await _service.ObtenerCatalogoDinamico("TipoVoz");

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(resultados);
        }
    }
}
