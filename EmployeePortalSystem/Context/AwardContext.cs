
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace EmployeePortalSystem.Context
{
    public class AwardContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AwardContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // This method will give you a live MySQL connection for Dapper to use
        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
