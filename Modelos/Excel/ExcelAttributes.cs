namespace Modelos.Excel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelHeaderAttribute(string nombre) : Attribute
    {
        public string Nombre { get; } = nombre;
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelFormatAttribute(string formato) : Attribute
    {
        public string Formato { get; } = formato;
    }

    public class ExcelOptions
    {
        public HashSet<string> Excluir { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Encabezados { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public string NombreHoja { get; set; } = "Datos";
    }
}
