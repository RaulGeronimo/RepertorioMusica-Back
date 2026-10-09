using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negocio;

namespace MusicaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyController : Controller
    {
        private readonly SpotifyService _spotify;

        public SpotifyController(SpotifyService spotify)
        {
            _spotify = spotify;
        }

        [HttpGet("Buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string q) => Ok(await _spotify.BuscarAsync(q));
    }
}
