using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Repositories
{
    public class SupportTicketRepository
    {
        private readonly AwardContext _context;

        public SupportTicketRepository(AwardContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(SupportTicket ticket)
        {
            var query = @"INSERT INTO SupportTickets (EmployeeId, IssueTitle, Description, Status, CreatedAt)
                          VALUES (@EmployeeId, @IssueTitle, @Description, @Status, @CreatedAt);
                          SELECT LAST_INSERT_ID();";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, ticket);
        }

        public async Task<IEnumerable<SupportTicketViewModel>> GetAllAsync()
        {
            var query = @"SELECT st.TicketId, st.IssueTitle, st.Description, st.Status, st.Response, st.CreatedAt,
                          emp.Name AS EmployeeName
                          FROM SupportTickets st
                          JOIN Employee emp ON emp.EmployeeId = st.EmployeeId";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SupportTicketViewModel>(query);
        }

        public async Task<IEnumerable<SupportTicketViewModel>> GetByEmployeeAsync(int employeeId)
        {
            var query = @"SELECT st.TicketId, st.IssueTitle, st.Description, st.Status, st.Response, st.CreatedAt,
                          emp.Name AS EmployeeName
                          FROM SupportTickets st
                          JOIN Employee emp ON emp.EmployeeId = st.EmployeeId
                          WHERE st.EmployeeId = @EmployeeId";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SupportTicketViewModel>(query, new { EmployeeId = employeeId });
        }
    }
}
