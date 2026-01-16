using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TaskFlow.Data
{
    public class AcessaDados
    {
        private readonly IConfiguration _configuration;

        public AcessaDados(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retorna uma conexão com o banco de dados local SQL Server Express
        /// </summary>
        public IDbConnection GetConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Retorna a string de conexão configurada
        /// </summary>
        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection");
        }
    }
}