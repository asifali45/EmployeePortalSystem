using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace EmployeePortalSystem.Repositories
{
    public class DepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

      

        public IEnumerable<Department> GetAll()
        {
            using var db = _context.CreateConnection();
            return db.Query<Department>("SELECT * FROM department");
        }

        public Department GetById(int id)
        {
            using var db = _context.CreateConnection();
            return db.QueryFirstOrDefault<Department>("SELECT * FROM department WHERE DepartmentId = @id", new { id });
        }

        public void Add(Department dept)
        {
            using var db = _context.CreateConnection();
            string sql = @"
                INSERT INTO department 
                    (Name, ParentDepartmentId, HeadId, Description, CreatedBy, CreatedAt)
                VALUES 
                    (@Name, @ParentDepartmentId, @HeadId, @Description, @CreatedBy, NOW())";
            db.Execute(sql, dept);
        }

        public void Update(Department dept)
        {
            using var db = _context.CreateConnection();
            string sql = @"
                UPDATE department SET
                    Name = @Name,
                    ParentDepartmentId = @ParentDepartmentId,
                    HeadId = @HeadId,
                    Description = @Description,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = NOW()
                WHERE DepartmentId = @DepartmentId";
            db.Execute(sql, dept);
        }

        public void Delete(int id)
        {
            using var db = _context.CreateConnection();
            db.Execute("DELETE FROM department WHERE DepartmentId = @id", new { id });
        }

        public List<DepartmentViewModel> GetAllWithDetails()
        {
            using var db = _context.CreateConnection();
            string sql = @"
                SELECT 
                    d.DepartmentId,
                    d.Name,
                    d.Description,
                    e.Name AS HeadName,
                    pd.Name AS ParentDepartmentName,
                    (SELECT COUNT(*) FROM employee emp WHERE emp.DepartmentId = d.DepartmentId) AS EmployeeCount
                FROM department d
                LEFT JOIN employee e ON d.HeadId = e.EmployeeId
                LEFT JOIN department pd ON d.ParentDepartmentId = pd.DepartmentId";

            return db.Query<DepartmentViewModel>(sql).ToList();
        }

        public List<EmployeeDetailsViewModel> GetEmployeesByDepartmentId(int departmentId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
            SELECT 
                e.EmployeeId,
                e.Name,
                e.Email,
                e.Phone,
                r.RoleName AS Designation,
                e.Photo
            FROM employee e
            LEFT JOIN role r ON e.RoleId = r.RoleId
            WHERE e.DepartmentId = @departmentId";

            return conn.Query<EmployeeDetailsViewModel>(sql, new { departmentId }).ToList();
        }


        public string? GetHeadNameById(int? headId)
        {
            if (headId == null || headId == 0)
                return null;

            using var db = _context.CreateConnection();
            return db.QueryFirstOrDefault<string>(
                "SELECT Name FROM employee WHERE EmployeeId = @id", new { id = headId });
        }

        public IEnumerable<Employee> SearchEmployeesByName(string term)
        {
            using var db = _context.CreateConnection();
            return db.Query<Employee>(
                "SELECT EmployeeId, Name FROM employee WHERE Name LIKE @term",
                new { term = "%" + term + "%" });
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            using var db = _context.CreateConnection();
            return db.Query<Employee>("SELECT EmployeeId, Name FROM employee");
        }

        public bool HasEmployees(int departmentId)
        {
            using var db = _context.CreateConnection();
            string sql = "SELECT COUNT(*) FROM employee WHERE DepartmentId = @id";
            return db.ExecuteScalar<int>(sql, new { id = departmentId }) > 0;
        }

    }
}
