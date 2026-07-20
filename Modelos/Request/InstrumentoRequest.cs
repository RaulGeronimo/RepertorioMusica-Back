namespace Modelos.Request
{
    public class InstrumentoRequest
    {
        public int InstrumentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
    }
}
