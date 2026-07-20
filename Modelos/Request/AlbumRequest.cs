namespace Modelos.Request
{
    public class AlbumRequest
    {
        public int AlbumId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int GrupoId { get; set; }
        public int DisqueraId { get; set; }
        public string Duracion { get; set; } = string.Empty;
        public DateTime Lanzamiento { get; set; }
        public string Grabacion { get; set; } = string.Empty;
        public string Portada { get; set; } = string.Empty;
    }
}
