using Microsoft.Extensions.Configuration;
using Modelos.Request;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Negocio
{
    public class SpotifyService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;
        private static string? _token;
        private static DateTime _expira = DateTime.MinValue;

        public SpotifyService(HttpClient http, IConfiguration cfg) { _http = http; _cfg = cfg; }

        private async Task<string> GetTokenAsync()
        {
            if (_token != null && DateTime.UtcNow < _expira) return _token;

            var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $"{_cfg["Spotify:ClientId"]}:{_cfg["Spotify:ClientSecret"]}"));

            var req = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") })
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            var res = await _http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var json = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement;

            _token = json.GetProperty("access_token").GetString();
            _expira = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32() - 60);
            return _token!;
        }

        public async Task<List<SpotifyTrack>> BuscarAsync(string q)
        {
            var req = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.spotify.com/v1/search?type=track&limit=5&q={Uri.EscapeDataString(q)}");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync());

            var res = await _http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var items = JsonDocument.Parse(await res.Content.ReadAsStringAsync())
                .RootElement.GetProperty("tracks").GetProperty("items");

            return items.EnumerateArray().Select(t =>
            {
                var ms = t.GetProperty("duration_ms").GetInt32();

                var portada = t.GetProperty("album").GetProperty("images")
                    .EnumerateArray()
                    .Select(i => i.GetProperty("url").GetString())
                    .LastOrDefault() ?? string.Empty;

                return new SpotifyTrack
                {
                    Id = t.GetProperty("id").GetString()!,
                    Nombre = t.GetProperty("name").GetString()!,
                    Artista = string.Join(", ", t.GetProperty("artists").EnumerateArray()
                                            .Select(a => a.GetProperty("name").GetString())),
                    Album = t.GetProperty("album").GetProperty("name").GetString()!,
                    Portada = portada,
                    Duracion = TimeSpan.FromMilliseconds(ms).ToString(@"mm\:ss")
                };
            }).ToList();
        }
    }
}
