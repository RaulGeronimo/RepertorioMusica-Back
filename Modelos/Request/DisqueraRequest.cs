namespace Modelos.Request
{
    public class DisqueraRequest
    {
        public int DisqueraId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fundacion { get; set; }
        public string Fundador { get; set; } = string.Empty;
        public string Generos { get; set; } = string.Empty;
        public int PaisId { get; set; }
        public int EstatusId { get; set; }
        public string Logo { get; set; } = string.Empty;
    }
}
