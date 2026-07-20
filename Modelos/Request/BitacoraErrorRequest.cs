namespace Modelos.Request
{
    public class BitacoraErrorRequest
    {
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public int UsuarioId { get; set; } = 0;
        public string Code { get; set; } = string.Empty;
    }
}
