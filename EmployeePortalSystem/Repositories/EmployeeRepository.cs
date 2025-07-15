using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using EmployeePortalSystem.Context;


namespace EmployeePortalSystem.Repositories
{
    public class EmployeeRepository
    {
        private readonly AppDbContext _context; 
        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public void AddEmployee(Employee emp)
        {
            using var conn=_context.CreateConnection();
                conn.Open();
                string sql = @"INSERT INTO employee(Name, Email, Phone, Photo, IsAdmin, DepartmentId, RoleId, CreatedBy, CreatedAt, UpdatedAt) 
                            VALUES (@Name, @Email, @Phone, @Photo, @IsAdmin, @DepartmentId, @RoleId, @CreatedBy, @CreatedAt, @UpdatedAt);";

                conn.Execute(sql, emp);
                
        }
        public List<Department> GetDepartments()
        {
            using var conn = _context.CreateConnection();
            return conn.Query<Department>("SELECT DepartmentId, Name FROM department").ToList();
        }
        public List<Role> GetRoles()
        {
            using var conn = _context.CreateConnection();
            return conn.Query<Role>("SELECT RoleId, RoleName FROM role").ToList();
        }

        public List<EmployeeDetailsViewModel> GetAllEmployeeDetails()
        {
            using var conn = _context.CreateConnection();

            conn.Open();
                string sql = @"
                    SELECT 
                        e.EmployeeId,
                        e.Name,
                        e.Email,
                        e.Phone,
                        e.Photo,
                        r.RoleName AS Designation,
                        d.Name AS DepartmentName
                    FROM employee e
                    LEFT JOIN role r ON e.RoleId = r.RoleId
                    LEFT JOIN department d ON e.DepartmentId = d.DepartmentId
                    WHERE e.IsCurrentEmployee = 1";
                return conn.Query<EmployeeDetailsViewModel>(sql).ToList();
            
        }

        public Employee GetEmployeeById(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = "SELECT * FROM employee WHERE EmployeeId = @Id";
            return conn.QueryFirstOrDefault<Employee>(sql, new { Id = id });
        }
        public void UpdateEmployee(Employee emp)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
        UPDATE employee SET 
            Name = @Name,
            Email = @Email,
            Phone = @Phone,
            Photo = @Photo,
            IsAdmin = @IsAdmin,
            DepartmentId = @DepartmentId,
            RoleId = @RoleId,
            UpdatedAt = @UpdatedAt,
            UpdatedBy = @UpdatedBy
        WHERE EmployeeId = @EmployeeId";

            conn.Execute(sql, emp);
        }

        public void DeleteEmployee(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = "UPDATE employee SET IsCurrentEmployee = 0 WHERE EmployeeId = @id";
            conn.Execute(sql, new { Id = id });
        }

        //used when announcemnt filtering in dashboard
        public int? GetDepartmentIdByEmployeeId(int empId)
        {
            using var conn = _context.CreateConnection();
            string sql = "SELECT DepartmentId FROM employee WHERE EmployeeId = @empId";
            return conn.QueryFirstOrDefault<int?>(sql, new { empId });
        }



    }

}

