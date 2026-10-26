using Microsoft.AspNetCore.Mvc;

namespace MusicaAPI.Extensions
{
    public static class ExcelResultExtensions
    {
        public const string ContentType =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static FileContentResult ExcelFile(this ControllerBase controller, byte[] bytes, string nombre)
            => controller.File(bytes, ContentType, $"{nombre}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }
}
