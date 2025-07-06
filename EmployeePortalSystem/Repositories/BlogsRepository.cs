using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Repositories
{
    public class BlogsRepository
    {
        private readonly AppDbContext _context;

        public BlogsRepository(AppDbContext context)
        {
            _context = context;
        }

        //public List<BlogViewModel> GetBlogDetails()
        //{
        //    using var connection=_context.CreateConnection();

        //    string sql = @"
        //            SELECT
        //                b.BlogId,
        //                b.Title,
        //                b.Content,
        //                b.Image,
        //                b.Tags,
        //                e.Name as AuthorName,
        //                b.CreatedAt,
        //                b.UpdatedAt
        //            FROM Blog b 
        //            LEFT JOIN Employee e ON b.AuthorId = e.EmployeeId";
        //    return connection.Query<BlogViewModel>(sql).ToList();
        //}

        public List<BlogViewModel> GetBlogDetails()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
        SELECT
            b.BlogId,
            b.Title,
            b.Content,
            b.Image,
            b.Tags,
            e.Name AS AuthorName,
            b.CreatedAt,
            b.UpdatedAt,
            (
                SELECT COUNT(*) 
                FROM blog_like bl 
                WHERE bl.BlogId = b.BlogId
            ) AS LikeCount
        FROM Blog b
        LEFT JOIN Employee e ON b.AuthorId = e.EmployeeId";

            return connection.Query<BlogViewModel>(sql).ToList();
        }

        public BlogViewModel GetBlogById(int id)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
        SELECT 
            b.BlogId,
            b.Title,
            b.Content,
            b.Image,
            b.Tags,
            b.CreatedAt,
            b.UpdatedAt,
            e.Name AS AuthorName
        FROM Blog b
        LEFT JOIN Employee e ON b.AuthorId = e.EmployeeId
        WHERE b.BlogId = @Id";
            return connection.QueryFirstOrDefault<BlogViewModel>(sql, new{ Id=id});
        }

        public void DeleteBlog(int id)
        {
            using var connection = _context.CreateConnection();
            string sql = "DELETE FROM Blog WHERE BlogId=@Id";
            connection.Execute(sql, new { Id = id });
        }

        public void ToggleLike(int blogId, int employeeId)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT COUNT(*) FROM blog_like WHERE BlogId=@BlogId AND EmployeeId=@EmployeeId";

            var alreadyLiked = connection.ExecuteScalar<int>(sql, new { BlogId = blogId, EmployeeId = employeeId }) > 0;

            if (alreadyLiked)
            {
                connection.Execute(
                          "DELETE FROM blog_like WHERE BlogId = @BlogId AND EmployeeId = @EmployeeId",
                           new { BlogId = blogId, EmployeeId = employeeId });
            }
            else
            {
                connection.Execute(
                "INSERT INTO blog_like (BlogId, EmployeeId) VALUES (@BlogId, @EmployeeId)",
                 new { BlogId = blogId, EmployeeId = employeeId });

            }
        }

    }
}
