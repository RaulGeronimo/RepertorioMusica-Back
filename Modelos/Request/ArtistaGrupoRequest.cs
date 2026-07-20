namespace Modelos.Request
{
    public class ArtistaGrupoRequest
    {
        public int ArtistaGrupoId { get; set; }
        public int ArtistaId { get; set; }
        public int GrupoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
