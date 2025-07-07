using EmployeePortalSystem.Context;
using EmployeePortalSystem.ViewModels;
using MySql.Data.MySqlClient;
using Dapper;

namespace EmployeePortalSystem.Repositories
{
    public class MyProfileRepository
    {
        private readonly AppDbContext _context;
        public MyProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public MyProfileViewModel GetEmployeeDetailsById(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
            SELECT 
                e.EmployeeId, e.Name, e.Email, e.Phone, e.Photo,
                r.RoleName AS Designation,
                d.Name AS DepartmentName
            FROM employee e
            LEFT JOIN role r ON e.RoleId = r.RoleId
            LEFT JOIN department d ON e.DepartmentId = d.DepartmentId
            WHERE e.EmployeeId = @id";

            return conn.QueryFirstOrDefault<MyProfileViewModel>(sql, new { id });
        }
        public List<string> GetCommitteesByEmployeeId(int employeeId)
        {
            using var conn = _context.CreateConnection();
            return conn.Query<string>(@"
        SELECT c.Name
        FROM committee_member cm
        JOIN committee c ON cm.CommitteeId = c.CommitteeId
        WHERE cm.EmployeeId = @employeeId", new { employeeId }).ToList();
        }
        public List<AwardViewModel> GetAwardsByEmployeeId(int empId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
        SELECT 
            a.AwardId,
            a.Type,
            a.EventDate,
            e.Name AS RecipientName,
            a.GivenBy,
            a.Description,
            a.DisplayOrder,
            a.CreatedBy,
            a.UpdatedBy,
            e.Photo AS RecipientPhoto
        FROM awards a
        JOIN employee e ON a.RecipientId = e.EmployeeId
        WHERE a.RecipientId = @empId";

            return conn.Query<AwardViewModel>(sql, new { empId }).ToList();
        }

        public List<BlogViewModel> GetBlogsByEmployeeId(int empId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
        SELECT 
            b.BlogId,
            b.Title,
            b.Content,
            b.Image,
            b.Tags,
            b.AuthorId,
            e.Name AS AuthorName,
            b.CreatedAt,
            b.UpdatedAt
        FROM blog b
        JOIN employee e ON b.AuthorId = e.EmployeeId
        WHERE b.AuthorId = @empId";

            return conn.Query<BlogViewModel>(sql, new { empId }).ToList();
        }

        public List<PollViewModel> GetPollsByEmployeeId(int empId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
        SELECT 
            p.PollId,
            p.Question,
            p.Option1,
            p.Option2,
            p.Option3,
            p.Option4,
            p.CreatedBy,
            e.Name AS CreatedByName,
            p.CreatedAt
        FROM poll_response pr
        JOIN polls p ON pr.PollId = p.PollId
        JOIN employee e ON p.CreatedBy = e.EmployeeId
        WHERE pr.EmployeeId = @empId";

            return conn.Query<PollViewModel>(sql, new { empId }).ToList();
        }



      
    }
}
