using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;


namespace EmployeePortalSystem.Repositories
{
    public class EmployeeRepository
    {
        private readonly string _connection; 
        public EmployeeRepository(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }
        public void AddEmployee(Employee emp)
        {
            using (var conn = new MySqlConnection(_connection)) 
            {
                conn.Open();
                string sql = @"INSERT INTO employee(Name, Email, Phone, Photo, IsAdmin, DepartmentId, RoleId, CreatedBy, CreatedAt, UpdatedAt) 
                            VALUES (@Name, @Email, @Phone, @Photo, @IsAdmin, @DepartmentId, @RoleId, @CreatedBy, @CreatedAt, @UpdatedAt);";



                conn.Execute(sql, emp);
            }
        }
        public List<Department> GetDepartments()
        {
            using var conn = new MySqlConnection(_connection);
            return conn.Query<Department>("SELECT DepartmentId, Name FROM department").ToList();
        }
        public List<Role> GetRoles()
        {
            using var conn = new MySqlConnection(_connection);
            return conn.Query<Role>("SELECT RoleId, RoleName FROM role").ToList();
        }

        public List<EmployeeDetailsViewModel> GetAllEmployeeDetails()
        {
            using var conn = new MySqlConnection(_connection);
            
                conn.Open();
                string sql = @"
                    SELECT 
                        e.EmployeeId,
                        e.Name,
                        e.Email,
                        e.Phone,
                        r.RoleName AS Designation,
                        d.Name AS DepartmentName
                    FROM employee e
                    LEFT JOIN role r ON e.RoleId = r.RoleId
                    LEFT JOIN department d ON e.DepartmentId = d.DepartmentId";
                return conn.Query<EmployeeDetailsViewModel>(sql).ToList();
            
        }

        public Employee GetEmployeeById(int id)
        {
            using var conn = new MySqlConnection(_connection);
            string sql = "SELECT * FROM employee WHERE EmployeeId = @Id";
            return conn.QueryFirstOrDefault<Employee>(sql, new { Id = id });
        }
        public void UpdateEmployee(Employee emp)
        {
            using var conn = new MySqlConnection(_connection);
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
            using var conn = new MySqlConnection(_connection);
            string sql = "DELETE FROM employee WHERE EmployeeId = @Id";
            conn.Execute(sql, new { Id = id });
        }


    }

}

