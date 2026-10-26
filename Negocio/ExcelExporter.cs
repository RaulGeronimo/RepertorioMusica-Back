using ClosedXML.Excel;
using Modelos.Excel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Negocio
{
    public class ExcelLibro
    {
        internal XLWorkbook Workbook { get; } = new();

        public ExcelLibro Hoja<T>(string nombre, IEnumerable<T> lista, ExcelOptions? opt = null)
        {
            opt ??= new ExcelOptions();
            opt.NombreHoja = nombre;
            ExcelExporter.AgregarHoja(Workbook, lista, opt);
            return this;
        }
    }

    public static class ExcelExporter
    {
        private static readonly HashSet<string> ExcluirGlobal = new(StringComparer.OrdinalIgnoreCase)
        {
            "TotalRegistros",
            "UsuarioId",
            "Activo",
            "RolId",
            "BitacoraCargaId",
            "BitacoraErrorId",

            "PaisId",
            "ArtistaId",
            "GrupoId",
            "DisqueraId",
            "AlbumId",
            "CancionId",
            "CancionAlbumId",
            "ArtistaGrupoId",
            "InstrumentoId",
            "InstrumentoArtistaGrupoId",
            "Foto",
            "Bandera",
            "Logo",
            "Portada",
            "SitioWeb",
            "SpotifyId",
        };

        private static readonly Dictionary<string, string> EncabezadosGlobal = new(StringComparer.OrdinalIgnoreCase)
        {
            //["FechaNacimiento"] = "Fecha Nacimiento",
            ["Registro"] = "Fecha Registro",
            ["DiasCumple"] = "Días Cumpleaños",
            //["NombreCompleto"] = "Nombre Completo",

            ["NombreArtistico"] = "Nombre Artístico",
            ["Duracion"] = "Duración",
            ["Grabacion"] = "Grabación",
            ["Pais"] = "País",
            ["Numero"] = "Número",
            ["Album"] = "Álbum",
            ["Albumes"] = "Álbumes",
            ["Publicacion"] = "Publicación",
            ["Cancion"] = "Canción",
            ["Interpretacion"] = "Interpretación",
            ["Fundacion"] = "Fundación",
            ["Descripcion"] = "Descripción",
        };

        private const string FormatoFecha = "dd/mm/yyyy";
        private const string FormatoFechaHora = "dd/mm/yyyy hh:mm";
        private static readonly Regex SepararMayusculas = new("(?<=[a-z0-9])(?=[A-Z])", RegexOptions.Compiled);

        private static readonly Regex CaracteresInvalidosHoja = new(@"[:\\/?*\[\]]", RegexOptions.Compiled);

        public static byte[] Generar<T>(IEnumerable<T> lista, ExcelOptions? opt = null)
        {
            opt ??= new ExcelOptions();
            using var wb = new XLWorkbook();
            AgregarHoja(wb, lista, opt);
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public static byte[] GenerarMultiple(Action<ExcelLibro> configurar)
        {
            var libro = new ExcelLibro();
            try
            {
                configurar(libro);
                using var ms = new MemoryStream();
                libro.Workbook.SaveAs(ms);
                return ms.ToArray();
            }
            finally
            {
                libro.Workbook.Dispose();
            }
        }

        public static void AgregarHoja<T>(XLWorkbook wb, IEnumerable<T> lista, ExcelOptions opt)
        {
            var ws = wb.Worksheets.Add(LimpiarNombreHoja(opt.NombreHoja));

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !ExcluirGlobal.Contains(p.Name)
                         && !opt.Excluir.Contains(p.Name)
                         && p.GetCustomAttribute<ExcelIgnoreAttribute>() is null)
                .ToList();

            // Encabezados
            for (int c = 0; c < props.Count; c++)
                ws.Cell(1, c + 1).Value = ObtenerEncabezado(props[c], opt);

            // Atributos de formato leídos una sola vez por propiedad
            var formatos = props
                .Select(p => p.GetCustomAttribute<ExcelFormatAttribute>()?.Formato)
                .ToArray();

            // Datos
            int fila = 2;
            foreach (var item in lista)
            {
                for (int c = 0; c < props.Count; c++)
                {
                    var valor = props[c].GetValue(item);
                    if (valor is null) continue;

                    var cell = ws.Cell(fila, c + 1);
                    EscribirCelda(cell, valor);

                    if (formatos[c] is not null)
                        cell.Style.NumberFormat.Format = formatos[c];
                }
                fila++;
            }

            // Estilo del encabezado
            var header = ws.Range(1, 1, 1, Math.Max(props.Count, 1));
            header.Style.Font.Bold = true;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (fila > 2)
                ws.Range(1, 1, fila - 1, props.Count).SetAutoFilter();

            ws.SheetView.FreezeRows(1);
            //ws.Columns().AdjustToContents(1, Math.Min(fila, 200));
            foreach (var col in ws.ColumnsUsed())
            {
                col.AdjustToContents(1, Math.Min(fila, 200));
                col.Width = Math.Clamp(col.Width + 4, 10, 60);
            }
        }

        private static void EscribirCelda(IXLCell cell, object valor)
        {
            switch (valor)
            {
                case DateTime dt when dt.Year < 1900:
                    cell.Value = dt.TimeOfDay == TimeSpan.Zero
                        ? dt.ToString("dd/MM/yyyy")
                        : dt.ToString("dd/MM/yyyy HH:mm");
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    break;
                case DateOnly d when d.Year < 1900:
                    cell.Value = d.ToString("dd/MM/yyyy");
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    break;
                case DateTime dt:
                    cell.Value = dt;
                    cell.Style.DateFormat.Format = dt.TimeOfDay == TimeSpan.Zero ? FormatoFecha : FormatoFechaHora;
                    break;
                case DateOnly d:
                    cell.Value = d.ToDateTime(TimeOnly.MinValue);
                    cell.Style.DateFormat.Format = FormatoFecha;
                    break;
                case TimeOnly t:
                    cell.Value = t.ToTimeSpan();
                    cell.Style.DateFormat.Format = "hh:mm";
                    break;
                case bool b:
                    cell.Value = b ? "Sí" : "No";
                    break;
                case decimal or double or float:
                    cell.Value = Convert.ToDouble(valor);
                    cell.Style.NumberFormat.Format = "#,##0.00";
                    break;
                case int or long or short or byte:
                    cell.Value = Convert.ToDouble(valor);
                    cell.Style.NumberFormat.Format = "0";
                    break;
                case Enum e:
                    cell.Value = e.ToString();
                    break;
                default:
                    cell.Value = valor.ToString();
                    break;
            }
        }

        private static string ObtenerEncabezado(PropertyInfo p, ExcelOptions opt)
        {
            if (opt.Encabezados.TryGetValue(p.Name, out var local)) return local;

            var attr = p.GetCustomAttribute<ExcelHeaderAttribute>();
            if (attr is not null) return attr.Nombre;

            if (EncabezadosGlobal.TryGetValue(p.Name, out var global)) return global;

            return SepararMayusculas.Replace(p.Name, " ");
        }

        private static string LimpiarNombreHoja(string nombre)
        {
            var s = CaracteresInvalidosHoja.Replace(nombre, "");
            return s.Length > 31 ? s[..31] : s;
        }
    }
}
