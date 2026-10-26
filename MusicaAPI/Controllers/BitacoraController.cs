using MusicaAPI.Extensions;
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
    public class BitacoraController : Controller
    {
        private readonly DynamicService _service;

        public BitacoraController(DynamicService service)
        {
            _service = service;
        }

        [HttpGet("Carga")]
        public async Task<IActionResult> Carga()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, BitacoraCargaListResponse<BitacoraCargaResponse>, BitacoraCargaResponse>("scObtenerBitacoraCarga");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Carga/Usuario")]
        public async Task<IActionResult> CargaUsuario()
        {
            int UsuarioId = _service.ResolverUsuarioId();
            var (response, lista) = await _service.EjecutarSPPorIdLista<BitacoraCargaListResponse<BitacoraCargaResponse>, BitacoraCargaResponse>("scObtenerBitacoraCargaUsuario", "@UsuarioId", UsuarioId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Carga/Exportar")]
        public async Task<IActionResult> ExportarCarga()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, BitacoraCargaListResponse<BitacoraCargaResponse>, BitacoraCargaResponse>("scObtenerBitacoraCarga");

            if (!response.Success)
                return BadRequest(response);

            if (lista == null || !lista.Any())
                return NotFound(new { mensaje = response.Message });

            var bytes = ExcelExporter.Generar(lista, new ExcelOptions
            {
                NombreHoja = "Bitacora"
            });

            return this.ExcelFile(bytes, "BitacoraCarga");
        }

        [HttpGet("Error")]
        public async Task<IActionResult> Error()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, BitacoraErrorListResponse<BitacoraErrorResponse>, BitacoraErrorResponse>("scObtenerBitacoraError");

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Error/Usuario")]
        public async Task<IActionResult> ErrorUsuario()
        {
            int UsuarioId = _service.ResolverUsuarioId();
            var (response, lista) = await _service.EjecutarSPPorIdLista<BitacoraErrorListResponse<BitacoraErrorResponse>, BitacoraErrorResponse>("scObtenerBitacoraErrorUsuario", "@UsuarioId", UsuarioId);

            if (!response.Success)
            { return BadRequest(response); }

            return Ok(lista);
        }

        [HttpGet("Error/Exportar")]
        public async Task<IActionResult> ExportarError()
        {
            var (response, lista) = await _service.EjecutarSPConXmlLista<object, BitacoraErrorListResponse<BitacoraErrorResponse>, BitacoraErrorResponse>("scObtenerBitacoraError");

            if (!response.Success)
                return BadRequest(response);

            if (lista == null || !lista.Any())
                return NotFound(new { mensaje = response.Message });

            var bytes = ExcelExporter.Generar(lista, new ExcelOptions
            {
                NombreHoja = "Bitacora"
            });

            return this.ExcelFile(bytes, "BitacoraError");
        }
    }
}
