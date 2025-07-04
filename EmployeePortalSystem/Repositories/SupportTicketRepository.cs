using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;


namespace EmployeePortalSystem.Repositories
{
    public class SupportTicketRepository
    {
        private readonly AwardContext _context; 

        public SupportTicketRepository(AwardContext context)
        {
            _context = context;
        }

        //  Create Ticket
        public async Task<int> CreateAsync(SupportTicket ticket)
        {

            var query = @"INSERT INTO support_tickets 
                        (EmployeeId, IssueTitle, Description, Status, CreatedAt)
                        VALUES 
                        (@EmployeeId, @IssueTitle, @Description, @Status, @CreatedAt);
                        SELECT LAST_INSERT_ID();";


            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, ticket);
        }

        //  Get All Tickets (Admin View)
        public async Task<IEnumerable<SupportTicket>> GetAllAsync()
        {
            var query = @"
    SELECT 
        t.*,
        emp.Name AS EmployeeName,
        assign.Name AS AssignedToName,
        escal.Name AS EscalatedToName
    FROM support_tickets t
    LEFT JOIN Employee emp ON t.EmployeeId = emp.EmployeeId
    LEFT JOIN Employee assign ON t.AssignedTo = assign.EmployeeId
    LEFT JOIN Employee escal ON t.EscalatedTo = escal.EmployeeId";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SupportTicket>(query);
        }

        //  Get Tickets by Employee ID
        public async Task<IEnumerable<SupportTicket>> GetByEmployeeAsync(int employeeId)
        {
            var query = @"SELECT * FROM support_tickets WHERE EmployeeId = @EmployeeId";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SupportTicket>(query, new { EmployeeId = employeeId });
        }

        //  Update Ticket (Assign, Escalate, Respond)
        public async Task<int> UpdateTicketAsync(SupportTicket ticket)
        {
            var query = @"UPDATE support_tickets SET 
                            Status = @Status,
                            Response = @Response,
                            AssignedTo = @AssignedTo,
                            EscalatedTo = @EscalatedTo,
                            EscalationLevel = @EscalationLevel,
                            UpdatedBy = @UpdatedBy,
                            UpdatedAt = @UpdatedAt
                        WHERE TicketId = @TicketId";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, ticket);
        }

        //  Get All Employee Names
        public async Task<IEnumerable<string>> GetAllEmployeeNamesAsync()
        {

            var query = "SELECT Name FROM Employee ORDER BY Name";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<string>(query);
        }


        //  Get Ticket by ID
        public async Task<SupportTicket?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM support_tickets WHERE TicketId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SupportTicket>(query, new { Id = id });
        }

        //  Get Employee ID from Name
        public async Task<int> GetEmployeeIdByNameAsync(string name)
        {

            var query = "SELECT EmployeeId FROM Employee WHERE Name = @Name LIMIT 1";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int?>(query, new { Name = name }) ?? 0;
        }

        //  Get Employee Name by ID
        public async Task<string> GetEmployeeNameByIdAsync(int? id)
        {
            if (id == null || id == 0)
                return "None";

            var query = "SELECT Name FROM Employee WHERE EmployeeId = @Id LIMIT 1";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<string>(query, new { Id = id }) ?? "None";
        }

       


        public async Task<List<SelectListItem>> GetAllDepartmentsAsync()
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT DepartmentId, DepartmentName FROM Department";

            var departments = await connection.QueryAsync<Department>(query);
            var list = departments.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.Name
            }).ToList();

            return list;
        }



        public async Task<List<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT EmployeeId, Name FROM Employee WHERE DepartmentId = @Id";
            return (await connection.QueryAsync<Employee>(query, new { Id = departmentId })).ToList();
        }





        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT * FROM Employee WHERE EmployeeId = @Id";
            return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = employeeId });
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT * FROM Department";
            return (await connection.QueryAsync<Department>(query)).ToList();
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT * FROM Employee WHERE DepartmentId = @Id";
            return (await connection.QueryAsync<Employee>(query, new { Id = departmentId })).ToList();
        }
    }
}
