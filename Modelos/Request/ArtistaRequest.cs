namespace Modelos.Request
{
    public class ArtistaRequest
    {
        public int ArtistaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreArtistico { get; set; } = string.Empty;
        public int GeneroId { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime? FechaFinado { get; set; }
        public decimal? Estatura { get; set; }
        public int PaisId { get; set; }
        public string Instrumentos { get; set; } = string.Empty;
        public int TipoVozId { get; set; }
        public string Foto { get; set; } = string.Empty;
    }
}
