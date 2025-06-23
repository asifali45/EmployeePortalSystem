using System.Collections.Generic;
using EmployeePortalSystem.Models;
using System.Data;
using Dapper;
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
            string sql = @"INSERT INTO department (Name, ParentDepartmentId, HeadId, Description, CreatedBy, CreatedAt)
                       VALUES (@Name, @ParentDepartmentId, @HeadId, @Description, @CreatedBy, NOW())";
            db.Execute(sql, dept);
        }

        public void Update(Department dept)
        {
            using var db = Connection;
            string sql = @"UPDATE department SET
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
    }

}