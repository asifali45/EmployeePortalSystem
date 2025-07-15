using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
namespace EmployeePortalSystem.Repositories
{
    public class RolesRepository
    {
        private readonly AppDbContext _context;
        public RolesRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<Role> GetAllRoles()
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            return conn.Query<Role>("SELECT RoleId, RoleName, CreatedAt FROM role").ToList();
        }
        public void AddRole(Role role)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            string sql = @"INSERT INTO role (RoleName, CreatedBy, CreatedAt)
            VALUES (@RoleName, @CreatedBy, @CreatedAt)";
            conn.Execute(sql, role);
        }
    }
}
