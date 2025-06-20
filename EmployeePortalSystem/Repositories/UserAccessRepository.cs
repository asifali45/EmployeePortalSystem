using EmployeePortalSystem.Models;
using MySql.Data.MySqlClient;
using Dapper;

namespace EmployeePortalSystem.Repositories
{
    public class UserAccessRepository
    {
        private readonly string _connectionString;

        public UserAccessRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Employee EmployeeLogin(string email,string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM employee WHERE email=@Email AND password=@Password";

                var employee = connection.QueryFirstOrDefault<Employee>(query, new { Email = email, Password = password });

                return employee;



            }
        }
    }

}
