using Microsoft.Extensions.Configuration;
using System.Data;

namespace Conexion
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class ConexionBD : IConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ConexionBD(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection CreateConnection()
        {
            return new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
        }
    }
}
