using MySql.Data.MySqlClient;
using Dapper;
using EmployeePortalSystem.ViewModels;
using System.Data.Common;

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
                SELECT e.EmployeeId, e.Name, r.RoleName, e.IsAdmin
                FROM employee e
                JOIN role r ON e.RoleId = r.RoleId
                WHERE e.Email = @Email AND e.Password = @Password";


                var employee = connection.QueryFirstOrDefault<LoginViewModel>(query, new { Email = email, Password = password });

                return employee;

            }
        }

        public DashboardCardViewModel GetCardCounts(int empid)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            string sql = @"
                SELECT 
            (SELECT COUNT(*) FROM awards WHERE RecipientId = @EmpId) AS TotalAwards,
            (SELECT COUNT(*) FROM blog WHERE AuthorId = @EmpId) AS BlogsWritten,
            (SELECT COUNT(*) FROM poll_response WHERE EmployeeId = @EmpId) AS PollsVoted;
             ";

            return conn.QueryFirst<DashboardCardViewModel>(sql, new { EmpId = empid });
        }

       

    }

}
