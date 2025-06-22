using MySql.Data.MySqlClient;
using Dapper;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Repositories
{
    public class UserAccessRepository
    {
        private readonly string _connectionString;

        public UserAccessRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public LoginViewModel EmployeeLogin(string email,string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT e.EmployeeId, e.Name, r.RoleName
                FROM employee e
                JOIN role r ON e.RoleId = r.RoleId
                WHERE e.Email = @Email AND e.Password = @Password";


                var employee = connection.QueryFirstOrDefault<LoginViewModel>(query, new { Email = email, Password = password });

                return employee;



            }
        }
    }

}
