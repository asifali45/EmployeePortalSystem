using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using Mysqlx.Crud;

namespace EmployeePortalSystem.Repositories
{
    public class BlogsRepository
    {
        private readonly AppDbContext _context;

        public BlogsRepository(AppDbContext context)
        {
            _context = context;
        }

       
        public List<BlogViewModel> GetBlogDetails(int employeeId)
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
        LEFT JOIN Employee e ON b.AuthorId = e.EmployeeId
         ORDER BY b.CreatedAt DESC"; 

            var blogs= connection.Query<BlogViewModel>(sql).ToList();

            // Load comments
            foreach (var blog in blogs)
            {
                blog.Comments = GetCommentsByBlogId(blog.BlogId.Value);
            }

            return blogs;
        }

        public void CreateBlog(BlogViewModel blog)
        {
            using var connection = _context.CreateConnection();
            string sql = @"INSERT INTO Blog(Title,Content,Image,Tags,AuthorId,CreatedAt,UpdatedAt)
                    VALUES(@Title,@Content,@Image,@Tags,@AuthorId,@CreatedAt,@UpdatedAt);";
            connection.Execute(sql, blog);

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
        public List<BlogCommentViewModel> GetCommentsByBlogId(int blogId)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
        SELECT c.CommentId, c.BlogId, c.EmployeeId, c.CommentText, c.CreatedAt, e.Name AS EmployeeName
        FROM blog_comment c
        LEFT JOIN Employee e ON c.EmployeeId = e.EmployeeId
        WHERE c.BlogId = @BlogId
        ORDER BY c.CreatedAt DESC";

            return connection.Query<BlogCommentViewModel>(sql, new { BlogId = blogId }).ToList();
        }

        public void AddComment(int blogId, int employeeId, string commentText)
        {
            using var connection = _context.CreateConnection();

            string sql = @"INSERT INTO blog_comment (BlogId, EmployeeId, CommentText) 
                   VALUES (@BlogId, @EmployeeId, @CommentText)";

            connection.Execute(sql, new { BlogId = blogId, EmployeeId = employeeId, CommentText = commentText });
        }


        //For blogs showing in the dashboard
        public List<BlogViewModel> GetLatestBlogsForDashboard(int count = 2)
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
    LEFT JOIN Employee e ON b.AuthorId = e.EmployeeId
    ORDER BY b.CreatedAt DESC
    LIMIT @Count";

            var blogs = connection.Query<BlogViewModel>(sql, new { Count = count }).ToList();

            foreach (var blog in blogs)
            {
                blog.Comments = GetCommentsByBlogId(blog.BlogId.Value);
            }

            return blogs;
        }



    }
}
