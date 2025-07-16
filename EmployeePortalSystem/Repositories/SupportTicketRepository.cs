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
        private readonly AppDbContext _context;

        public SupportTicketRepository(AppDbContext context)
        {
            _context = context;
        }

        //  Create Ticket
        public async Task<int> CreateAsync(SupportTicket ticket)
        {


            var query = @"INSERT INTO support_tickets 
                        (EmployeeId, IssueTitle,Type, Description, Status, CreatedAt)
                        VALUES 
                        (@EmployeeId, @IssueTitle, @Type, @Description, @Status, @CreatedAt);
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
                     LEFT JOIN Employee escal ON t.EscalatedTo = escal.EmployeeId
                    ORDER BY t.UpdatedAt DESC";
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

       
        //  Get All Employee Names
        public async Task<IEnumerable<string>> GetAllEmployeeNamesAsync()
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT Name FROM Employee ORDER BY Name";
            
            return await connection.QueryAsync<string>(query);
        }


        //  Get Ticket by ID
        public async Task<SupportTicket?> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT * FROM support_tickets WHERE TicketId = @Id";
            
            return await connection.QueryFirstOrDefaultAsync<SupportTicket>(query, new { Id = id });
        }

        //  Get Employee ID from Name
        public async Task<int> GetEmployeeIdByNameAsync(string name)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT EmployeeId FROM Employee WHERE Name = @Name LIMIT 1";
           
            return await connection.QueryFirstOrDefaultAsync<int?>(query, new { Name = name }) ?? 0;
        }

        public async Task<string> GetEmployeeNameById(int? employeeId)
        {
            if (employeeId == null) return "";

            var sql = "SELECT Name FROM employee WHERE EmployeeId = @EmployeeId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<string>(sql, new { EmployeeId = employeeId });
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
            {
                var query = "SELECT DepartmentId, DepartmentName FROM Department";

                var departments = await connection.QueryAsync<Department>(query);
                var list = departments.Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                }).ToList();

                return list;
            }
        }


        
        public async Task<List<Employee>> GetEmployeesByDepartmentIdAsync(int departmentId)
        {
            using var connection = _context.CreateConnection();
            {
                var query = "SELECT EmployeeId, Name FROM Employee WHERE DepartmentId = @Id";
                return (await connection.QueryAsync<Employee>(query, new { Id = departmentId })).ToList();
            }
        }

        //for employee ticket history
        public async Task<List<SupportTicketViewModel>> GetTicketsByEmployeeIdAsync(int empId)
        {
            using var connection = _context.CreateConnection();

            var query = @"
        SELECT 
            st.TicketId,
            st.IssueTitle,
            st.Description,
            st.Status,
            st.Response,
            e1.Name AS AssignedToName,
            e2.Name AS EscalatedToName
        FROM support_tickets st
        LEFT JOIN employee e1 ON st.AssignedTo = e1.EmployeeId
        LEFT JOIN employee e2 ON st.EscalatedTo = e2.EmployeeId
        WHERE st.EmployeeId = @EmpId";

            var tickets = await connection.QueryAsync<SupportTicketViewModel>(query, new { EmpId = empId });
            return tickets.ToList();
        }








        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            {
                var query = "SELECT * FROM Employee WHERE EmployeeId = @Id";
                return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = employeeId });
            }
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            using var connection = _context.CreateConnection();
            {
                var query = "SELECT * FROM Department";
                return (await connection.QueryAsync<Department>(query)).ToList();
            }
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            using var connection = _context.CreateConnection();
            {
                var query = "SELECT EmployeeId, Name, DepartmentId FROM Employee WHERE @Id = 0 OR DepartmentId = @Id";
                return (await connection.QueryAsync<Employee>(query, new { Id = departmentId })).ToList();
              }
        }
        public async Task<int> UpdateTicketAsync(SupportTicket ticket)
        {
            var query = @"UPDATE support_tickets SET 
                    Status = @Status,
                    Response = @Response,
                    AssignedTo = @AssignedTo,
                    EscalatedTo = @EscalatedTo,
                    EscalationLevel = @EscalationLevel,
                    Resolved = @Resolved,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                  WHERE TicketId = @TicketId";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, ticket);
        }
        //AssignedTickets
        public async Task<IEnumerable<SupportTicket>> GetAssignedTicketsAsync(int employeeId)
        {
            using var connection = _context.CreateConnection();
            var query = @"SELECT * FROM support_tickets WHERE AssignedTo = @employeeId";
            return await connection.QueryAsync<SupportTicket>(query, new { EmployeeId = employeeId });
        }
        // for new resolved 
        public async Task UpdateResolvedAsync(int ticketId, string resolved, int updatedBy)
        {
            using var connection = _context.CreateConnection();
            var query = @"
        UPDATE support_tickets 
        SET Resolved = @Resolved, UpdatedBy = @UpdatedBy, UpdatedAt = NOW()
        WHERE TicketId = @TicketId";

            await connection.ExecuteAsync(query, new
            {
                TicketId = ticketId,
                Resolved = resolved,
                UpdatedBy = updatedBy
            });
        }

    }
}
