using MySql.Data.MySqlClient;
using Dapper;
using EmployeePortalSystem.ViewModels;
using System.Data.Common;
using EmployeePortalSystem.Context;
using System;

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
                SELECT e.EmployeeId, e.Name, r.RoleName, e.IsAdmin, e.Photo
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

        public Dictionary<string, int> GetMonthlyContributionForEmployee(int employeeId)
        {
            using var conn = _context.CreateConnection();
            conn.Open();

            string sql = @"
                SELECT 'Blogs' AS Activity, COUNT(*) AS Count
                FROM blog
                WHERE AuthorId = @EmpId AND MONTH(CreatedAt) = MONTH(CURRENT_DATE()) AND YEAR(CreatedAt) = YEAR(CURRENT_DATE())
    
                UNION ALL
    
                SELECT 'Polls' AS Activity, COUNT(*) AS Count
                FROM poll_response
                WHERE EmployeeId = @EmpId AND MONTH(CreatedAt) = MONTH(CURRENT_DATE()) AND YEAR(CreatedAt) = YEAR(CURRENT_DATE())
    
                UNION ALL
    
                SELECT 'Awards' AS Activity, COUNT(*) AS Count
                FROM awards
                WHERE RecipientId = @EmpId AND MONTH(CreatedAt) = MONTH(CURRENT_DATE()) AND YEAR(CreatedAt) = YEAR(CURRENT_DATE())
            ";

            var results = conn.Query(sql, new { EmpId = employeeId });

            return results.ToDictionary(r => (string)r.Activity, r => (int)r.Count);
        }

        public Dictionary<string, int> GetDepartmentMemberCounts()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
        SELECT d.Name, COUNT(e.EmployeeId) AS MemberCount
        FROM department d
        LEFT JOIN employee e ON d.DepartmentId = e.DepartmentId
        GROUP BY d.Name
        ORDER BY d.Name;";

            var result = connection.Query(sql).ToDictionary(
                row => (string)row.Name,
                row => (int)row.MemberCount
            );

            return result;
        }

        public List<ContributorStats> GetTopContributors(int topN = 5)
        {
            using var db = _context.CreateConnection();
            var sql = @"
    SELECT 
        e.Name AS EmployeeName,
        COALESCE(b.BlogCount, 0) AS Blogs,
        COALESCE(p.PollCount, 0) AS Polls,
        COALESCE(a.AwardCount, 0) AS Awards
    FROM employee e
    LEFT JOIN (
        SELECT AuthorId, COUNT(*) AS BlogCount 
        FROM blog 
        GROUP BY AuthorId
    ) b ON e.EmployeeId = b.AuthorId
    LEFT JOIN (
        SELECT CreatedBy, COUNT(*) AS PollCount 
        FROM polls 
        GROUP BY CreatedBy
    ) p ON e.EmployeeId = p.CreatedBy
    LEFT JOIN (
        SELECT RecipientId, COUNT(*) AS AwardCount 
        FROM awards 
        GROUP BY RecipientId
    ) a ON e.EmployeeId = a.RecipientId
    ORDER BY (COALESCE(b.BlogCount, 0) + COALESCE(p.PollCount, 0) + COALESCE(a.AwardCount, 0)) DESC
    LIMIT @TopN";

            return db.Query<ContributorStats>(sql, new { TopN = topN }).ToList();
        }


    }

}
