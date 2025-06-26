using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace EmployeePortalSystem.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly string _connectionString;

        public DepartmentRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public IEnumerable<Department> GetAll()
        {
            using var db = Connection;
            return db.Query<Department>("SELECT * FROM department");
        }

        public Department GetById(int id)
        {
            using var db = Connection;
            return db.QueryFirstOrDefault<Department>("SELECT * FROM department WHERE DepartmentId = @id", new { id });
        }

        public void Add(Department dept)
        {
            using var db = Connection;
            string sql = @"
                INSERT INTO department 
                    (Name, ParentDepartmentId, HeadId, Description, CreatedBy, CreatedAt)
                VALUES 
                    (@Name, @ParentDepartmentId, @HeadId, @Description, @CreatedBy, NOW())";
            db.Execute(sql, dept);
        }

        public void Update(Department dept)
        {
            using var db = Connection;
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
            using var db = Connection;
            db.Execute("DELETE FROM department WHERE DepartmentId = @id", new { id });
        }

        // ✅ ViewModel data for Index
        public List<DepartmentViewModel> GetAllWithDetails()
        {
            using var db = Connection;
            string sql = @"
                SELECT 
                    d.DepartmentId,
                    d.Name,
                    d.Description,
                    e.Name AS HeadName,
                    pd.Name AS ParentDepartmentName
                FROM department d
                LEFT JOIN employee e ON d.HeadId = e.EmployeeId
                LEFT JOIN department pd ON d.ParentDepartmentId = pd.DepartmentId";

            return db.Query<DepartmentViewModel>(sql).ToList();
        }

        // ✅ NEW: Get Head Name by HeadId
        public string? GetHeadNameById(int? headId)
        {
            if (headId == null || headId == 0)
                return null;

            using var db = Connection;
            return db.QueryFirstOrDefault<string>(
                "SELECT Name FROM employee WHERE EmployeeId = @id", new { id = headId });
        }

        public IEnumerable<Employee> SearchEmployeesByName(string term)
        {
            using var db = Connection;
            return db.Query<Employee>(
                "SELECT EmployeeId, Name FROM employee WHERE Name LIKE @term",
                new { term = "%" + term + "%" });
        }


    }
}
