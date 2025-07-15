using MySql.Data.MySqlClient;
using Dapper;
using EmployeePortalSystem.ViewModels;
using System.Data.Common;
using EmployeePortalSystem.Context;

namespace EmployeePortalSystem.Repositories
{
    public class UserAccessRepository
    {
        private readonly AppDbContext _context;

        public UserAccessRepository(AppDbContext context)
        {
            _context = context;
        }



        public LoginViewModel EmployeeLogin(string email,string password)
        {
            using (var connection =_context.CreateConnection())
            {
                connection.Open();
                string query = @"
                SELECT e.EmployeeId, e.Name, r.RoleName, e.IsAdmin
                FROM employee e
                JOIN role r ON e.RoleId = r.RoleId
                WHERE e.Email = @Email AND e.Password = @Password AND e.IsCurrentEmployee = 1";


                var employee = connection.QueryFirstOrDefault<LoginViewModel>(query, new { Email = email, Password = password });

                return employee;

            }
        }

        public DashboardCardViewModel GetCardCounts(int empid)
        {
            using var conn =_context.CreateConnection();
            conn.Open();
            string sql = @"
                SELECT 
            (SELECT COUNT(*) FROM awards WHERE RecipientId = @EmpId) AS TotalAwards,
            (SELECT COUNT(*) FROM blog WHERE AuthorId = @EmpId) AS BlogsWritten,
            (SELECT COUNT(*) FROM poll_response WHERE EmployeeId = @EmpId) AS PollsVoted;
             ";

            return conn.QueryFirst<DashboardCardViewModel>(sql, new { EmpId = empid });
        }
        public bool UpdatePasswordIfEmailExists(string email, string newPassword)
        {
            using var conn = _context.CreateConnection();
            conn.Open();

            string checkSql = "SELECT COUNT(*) FROM employee WHERE Email = @Email AND IsCurrentEmployee = 1";
            int count = conn.ExecuteScalar<int>(checkSql, new { Email = email });

            if (count == 0)
                return false;

            string updateSql = "UPDATE employee SET Password = @Password WHERE Email = @Email";
            conn.Execute(updateSql, new { Email = email, Password = newPassword });

            return true;
        }


    }

}
