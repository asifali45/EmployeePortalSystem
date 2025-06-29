using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.ViewModels;
using EmployeePortalSystem.Models;

namespace EmployeePortalSystem.Repositories
{
    public class CommitteeRepository
    {
        private readonly AwardContext _context;

        public CommitteeRepository(AwardContext context)
        {
            _context = context;
        }

        public List<CommitteeViewModel> GetAllCommittees()
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                    SELECT
                        c.CommitteeId,
                        c.Name,
                        c.Type,
                        c.Logo,
                        c.Description,
                        e.Name AS HeadName
                        FROM Committee c
                        LEFT JOIN Employee e ON c.HeadId=e.EmployeeId";
            return connection.Query<CommitteeViewModel>(sql).ToList();
        }

        public List<Employee> GetAllEmployees()
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT EmployeeId,Name FROM Employee";
            return connection.Query<Employee>(sql).ToList();
        }

        public void CreateCommittee(Committee committee)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
                    INSERT INTO Committee(Name,Type,HeadId,Logo,Description,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt)
                    VALUES(@Name,@Type,@HeadId,@Logo,@Description,@CreatedBy,@CreatedAt,@UpdatedBy,@UpdatedAt);";
            connection.Execute(sql, committee);

        }
    }
}
