using EmployeePortalSystem.Context;
using EmployeePortalSystem.ViewModels;
using MySql.Data.MySqlClient;
using Dapper;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.X509;
using EmployeePortalSystem.Models;

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

            // First: Get blogs with LikeCount
            string blogSql = @"
        SELECT 
            b.BlogId,
            b.Title,
            b.Content,
            b.Image,
            b.Tags,
            b.AuthorId,
            e.Name AS AuthorName,
            b.CreatedAt,
            b.UpdatedAt,
            (SELECT COUNT(*) FROM blog_like bl WHERE bl.BlogId = b.BlogId) AS LikeCount
        FROM blog b
        JOIN employee e ON b.AuthorId = e.EmployeeId
        WHERE b.AuthorId = @empId
        ORDER BY b.CreatedAt DESC";

            var blogs = conn.Query<BlogViewModel>(blogSql, new { empId }).ToList();

            // Second: Get comments for all blogs
            string commentSql = @"
        SELECT 
            bc.CommentId,
            bc.BlogId,
            bc.EmployeeId,
            bc.CommentText,
            bc.CreatedAt,
            e.Name AS EmployeeName
        FROM blog_comment bc
        JOIN employee e ON bc.EmployeeId = e.EmployeeId
        WHERE bc.BlogId IN @BlogIds";

            var blogIds = blogs.Select(b => b.BlogId).ToList();

            if (blogIds.Any())
            {
                var comments = conn.Query<BlogCommentViewModel>(commentSql, new { BlogIds = blogIds }).ToList();

                // Map comments to respective blogs
                foreach (var blog in blogs)
                {
                    blog.Comments = comments.Where(c => c.BlogId == blog.BlogId).ToList();
                }
            }

            return blogs;
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
       FROM polls p
        JOIN employee e ON p.CreatedBy = e.EmployeeId
        WHERE p.CreatedBy = @empId
        ORDER BY p.CreatedAt DESC";

            return conn.Query<PollViewModel>(sql, new { empId }).ToList();
        }


        public Dictionary<int, string> GetSelectedOptionsByEmployee(int empId)
        {
            using var conn = _context.CreateConnection();
            var results = conn.Query<(int PollId, string SelectedOption)>(
                "SELECT PollId, SelectedOption FROM poll_response WHERE EmployeeId = @empId",
                new { empId }).ToList();

            return results.ToDictionary(r => r.PollId, r => r.SelectedOption);
        }

        public Employee GetEmployeeById(int empId)
        {
            using var conn = _context.CreateConnection();
            return conn.QueryFirstOrDefault<Employee>(
                "SELECT EmployeeId, Name, Photo FROM employee WHERE EmployeeId = @empId", new { empId });
        }

        public void UpdateEmployeePhoto(int empId, string fileName)
        {
            using var conn = _context.CreateConnection();
            conn.Execute("UPDATE employee SET Photo = @fileName WHERE EmployeeId = @empId", new { fileName, empId });
        }

    }
}
