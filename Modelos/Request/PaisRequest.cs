namespace Modelos.Request
{
    public class PaisRequest
    {
        public int PaisId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public int ContinenteId { get; set; }
        public string Bandera { get; set; } = string.Empty;
    }
}
