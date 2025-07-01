using System.Data;
using MySql.Data.MySqlClient;

namespace EmployeePortalSystem.Context
{
    public class AppDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
