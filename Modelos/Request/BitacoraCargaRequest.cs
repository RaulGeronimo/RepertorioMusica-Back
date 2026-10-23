namespace Modelos.Request
{
    public class BitacoraCargaRequest
    {
        public int SeccionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int UsuarioId { get; set; } = 0;
        public DateTime Modificado { get; set; } = DateTime.Now;
        public int ProcesoBitacoraId { get; set; }
        public int RegistroId { get; set; } = 0;
    }
}
