using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Modelos.Enums;
using Modelos.Request;
using Modelos.Response;
using System.Data;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Negocio
{
    public class DynamicService
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DynamicService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _httpContextAccessor = httpContextAccessor;
        }

        //private int UsuarioId => int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UsuarioId")?.Value, out var id) ? id : 1;

        //Métodos POST, PUT y DELETE
        public async Task<GenericResponse> EjecutarSPConXml<T>(string storedProcedure, T request)
        {
            string xml = XmlHelper.SerializeToXml(request);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@xml", xml);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (reader.Read())
            {
                string xmlContent = reader.GetString(0)?.Trim();

                try
                {
                    using var textReader = new StringReader(xmlContent);
                    var serializer = new XmlSerializer(typeof(GenericResponse));
                    var result = (GenericResponse)serializer.Deserialize(textReader)!;
                    return result;
                }
                catch (Exception ex)
                {
                    return new GenericResponse
                    {
                        Success = false,
                        Message = $"Error al deserializar XML: {ex.Message}",
                        Result = null,
                        Code = "500"
                    };
                }
            }

            return new GenericResponse
            {
                Success = false,
                Message = "No se obtuvo respuesta XML.",
                Code = "500"
            };
        }

        // GET ALL
        public async Task<(GenericResponse response, List<T>? lista)> EjecutarSPConXmlLista<TRequest, TLista, T>(string storedProcedure, TRequest? request = default) where TLista : class
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (request != null)
            {
                string xml = XmlHelper.SerializeToXml(request);
                var parametroXml = new SqlParameter("@xml", SqlDbType.Xml) { Value = xml };
                command.Parameters.Add(parametroXml);
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (reader.Read())
            {
                string xmlContent = reader.GetString(0)?.Trim()!;

                try
                {
                    var serializer = new XmlSerializer(typeof(GenericResponse));
                    using var textReader = new StringReader(xmlContent);
                    var response = (GenericResponse)serializer.Deserialize(textReader)!;

                    List<T>? lista = null;

                    if (response.Result != null)
                    {
                        var listSerializer = new XmlSerializer(typeof(TLista));
                        using var resultReader = new StringReader(response.Result.OuterXml);
                        var wrapper = (TLista)listSerializer.Deserialize(resultReader)!;

                        var property = typeof(TLista).GetProperty("Items");
                        if (property != null)
                        {
                            var value = property.GetValue(wrapper);
                            if (value is IEnumerable<T> enumerable)
                            {
                                lista = enumerable.ToList();
                            }
                        }
                    }

                    return (response, lista);
                }
                catch (Exception ex)
                {
                    return (new GenericResponse
                    {
                        Success = false,
                        Message = $"Error al deserializar XML: {ex.Message}",
                        Result = null,
                        Code = "500"
                    }, null);
                }
            }

            return (new GenericResponse
            {
                Success = false,
                Message = "No se obtuvo respuesta XML.",
                Code = "500"
            }, null);
        }

        //GET BY ID
        public async Task<(GenericResponse response, T? item)> EjecutarSPPorId<TLista, T>(string storedProcedure, string parametro, object valor) where TLista : class
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue(parametro, valor);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (reader.Read())
            {
                string xmlContent = reader.GetString(0)?.Trim()!;

                try
                {
                    var serializer = new XmlSerializer(typeof(GenericResponse));
                    using var textReader = new StringReader(xmlContent);
                    var response = (GenericResponse)serializer.Deserialize(textReader)!;

                    T? item = default;

                    if (response.Result != null)
                    {
                        var listSerializer = new XmlSerializer(typeof(TLista));
                        using var resultReader = new StringReader(response.Result.OuterXml);
                        var wrapper = (TLista)listSerializer.Deserialize(resultReader)!;

                        var property = typeof(TLista).GetProperty("Items");
                        if (property != null)
                        {
                            var value = property.GetValue(wrapper);
                            if (value is IEnumerable<T> enumerable)
                            {
                                item = enumerable.FirstOrDefault();
                            }
                        }
                    }

                    return (response, item);
                }
                catch (Exception ex)
                {
                    return (new GenericResponse
                    {
                        Success = false,
                        Message = $"Error al deserializar XML: {ex.Message}",
                        Result = null,
                        Code = "500"
                    }, default);
                }
            }

            return (new GenericResponse
            {
                Success = false,
                Message = "No se obtuvo respuesta XML.",
                Code = "500"
            }, default);
        }

        public async Task<(GenericResponse response, List<T>? lista)> EjecutarSPPorIdLista<TLista, T>(string storedProcedure, string parametro, object valor) where TLista : class
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue(parametro, valor);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (reader.Read())
            {
                string xmlContent = reader.GetString(0)?.Trim()!;

                try
                {
                    var serializer = new XmlSerializer(typeof(GenericResponse));
                    using var textReader = new StringReader(xmlContent);
                    var response = (GenericResponse)serializer.Deserialize(textReader)!;

                    List<T>? lista = null;

                    if (response.Result != null)
                    {
                        var listSerializer = new XmlSerializer(typeof(TLista));
                        using var resultReader = new StringReader(response.Result.OuterXml);
                        var wrapper = (TLista)listSerializer.Deserialize(resultReader)!;

                        var property = typeof(TLista).GetProperty("Items");
                        if (property != null)
                        {
                            var value = property.GetValue(wrapper);
                            if (value is IEnumerable<T> enumerable)
                            {
                                lista = enumerable.ToList();
                            }
                        }
                    }

                    return (response, lista);
                }
                catch (Exception ex)
                {
                    return (new GenericResponse
                    {
                        Success = false,
                        Message = $"Error al deserializar XML: {ex.Message}",
                        Code = "500"
                    }, null);
                }
            }

            return (new GenericResponse
            {
                Success = false,
                Message = "No se obtuvo respuesta XML.",
                Code = "500"
            }, null);
        }

        public async Task<(GenericResponse response, List<Dictionary<string, object>>? resultados)> ObtenerCatalogoDinamico(string nombreTabla, string? campo = null, object? valor = null, string columnas = "*")
        {
            var response = new GenericResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(nombreTabla))
                {
                    response.Success = false;
                    response.Message = "Nombre de tabla no válido.";
                    return (response, null);
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = $"SELECT {columnas} FROM {nombreTabla}";

                if (!string.IsNullOrWhiteSpace(campo) && valor != null)
                {
                    query += $" WHERE {campo} = @valor";
                }

                query += $" ORDER BY {campo ?? nombreTabla}";

                using var command = new SqlCommand(query, connection);

                if (!string.IsNullOrWhiteSpace(campo) && valor != null)
                {
                    command.Parameters.AddWithValue("@valor", valor);
                }

                using var reader = await command.ExecuteReaderAsync();

                var listaResultados = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var fila = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var nombreColumna = reader.GetName(i);
                        var valorColumna = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        fila[nombreColumna] = valorColumna!;
                    }

                    listaResultados.Add(fila);
                }

                response.Success = true;
                return (response, listaResultados);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error al obtener los datos: {ex.Message}";
                return (response, null);
            }
        }

        #region Bitacoras
        public async Task RegistrarBitacoraCarga(SeccionBitacora seccion, string nombre, ProcesoBitacora proceso, int registroId, int? usuarioId = null)
        {
            var bitacora = new BitacoraCargaRequest
            {
                SeccionId = (int)seccion,
                Nombre = nombre,
                UsuarioId = ResolverUsuarioId(usuarioId),
                Modificado = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                ProcesoBitacoraId = (int)proceso,
                RegistroId = registroId
            };

            await EjecutarSPConXml("scRegistrarBitacoraCarga", bitacora);
        }

        public async Task<string> RegistrarBitacoraError(string code, int? usuarioId = null)
        {
            var bitacora = new BitacoraErrorRequest
            {
                UsuarioId = ResolverUsuarioId(usuarioId),
                Code = code,
                FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
            };

            await EjecutarSPConXml("scRegistrarBitacoraError", bitacora);

            var request = new ErrorRequest
            {
                Code = code
            };


            var (response, errores) = await EjecutarSPConXmlLista<ErrorRequest,
                ErroresListResponse<ErrorResponse>, ErrorResponse>("scBuscarBitacoraError", request);

            var error = errores?.FirstOrDefault();
            return error?.MensajeError ?? "Ocurrió un error inesperado.";
        }
        #endregion Bitacoras

        #region Obtener UsuarioId
        public int? UsuarioIdActual => int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UsuarioId")?.Value, out var id) ? id : null;

        public int ResolverUsuarioId(int? usuarioIdOverride = null)
        {
            // 1. Usuario del token
            if (UsuarioIdActual.HasValue)
                return UsuarioIdActual.Value;

            // 2. Usuario encontrado manualmente
            if (usuarioIdOverride.HasValue)
                return usuarioIdOverride.Value;

            // 3. Admin por defecto
            return 1;
        }
        #endregion Obtener UsuarioId
    }
}
