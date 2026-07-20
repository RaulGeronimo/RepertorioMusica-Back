namespace Modelos.Request
{
    public class CancionRequest
    {
        public int CancionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public DateTime Publicacion { get; set; }
        public string Genero { get; set; } = string.Empty;
        public int InterpretacionId { get; set; }
        public int GrupoId { get; set; }
    }
}
