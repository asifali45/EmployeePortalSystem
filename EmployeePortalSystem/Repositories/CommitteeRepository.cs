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

        public Committee GetCommitteeById(int cid)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT * FROM committee WHERE CommitteeId=@Id";
            return connection.QueryFirstOrDefault<Committee>(sql, new { Id = cid });

        }

        public void DeleteCommittee(int cid)
        {
            using var connection = _context.CreateConnection();
            string sql = "DELETE FROM committee WHERE CommitteeId=@Id";
            connection.Execute(sql, new { Id = cid });
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

        public void UpdateCommittee(Committee committee)
        {
            using var connection = _context.CreateConnection();

            string sql = @"
        UPDATE Committee
        SET Name = @Name,
            Type = @Type,
            HeadId = @HeadId,
            Logo = @Logo,
            Description = @Description,
            UpdatedBy = @UpdatedBy,
            UpdatedAt = @UpdatedAt
        WHERE CommitteeId = @CommitteeId";

            connection.Execute(sql, committee);
        }

        public List<CommitteeMemberViewModel> GetCommitteeMembersByCommitteeId(int CommitteeId)
        { 
            using var connection = _context.CreateConnection();
            string sql = @"
                SELECT 
                    cm.CommitteeMemberId,
                    e.EmployeeId,
                    e.Name AS EmployeeName,
                    r.RoleName,
                    d.Name AS DepartmentName
                FROM committee_member cm
                INNER JOIN Employee e ON cm.EmployeeId = e.EmployeeId
                INNER JOIN Role r ON cm.RoleId = r.RoleId
                INNER JOIN Department d ON e.DepartmentId = d.DepartmentId
                WHERE cm.CommitteeId = @committeeId";
            return connection.Query<CommitteeMemberViewModel>(sql, new {CommitteeId}).ToList();
        }
    }
}
