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

        public List<BlogViewModel> GetBlogDetails()
        {
            using var connection=_context.CreateConnection();

            string sql = @"
                    SELECT
                        b.Title,
                        b.Content,
                        b.Image,
                        b.Tags,
                        e.Name as AuthorName,
                        b.CreatedAt,
                        b.UpdatedAt
                    FROM Blog b 
                    LEFT JOIN Employee e ON b.AuthorId = e.EmployeeId";
            return connection.Query<BlogViewModel>(sql).ToList();
        }
    }
}
