namespace Modelos.Request
{
    public class GrupoRequest
    {
        public int GrupoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
        public string Sellos { get; set; } = string.Empty;
        public int EstatusId { get; set; }
        public string SitioWeb { get; set; } = string.Empty;
        public int IdiomaId { get; set; }
        public string Logo { get; set; } = string.Empty;
    }
}
