using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.ViewModels;
using EmployeePortalSystem.Models;
using System;

namespace EmployeePortalSystem.Repositories
{
    public class CommitteeRepository
    {
        private readonly AppDbContext _context;

        public CommitteeRepository(AppDbContext context)
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
                        e.Name AS HeadName,
                        (SELECT COUNT(*) FROM committee_member cm WHERE cm.CommitteeId = c.CommitteeId) AS MemberCount
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
            string sql = "SELECT EmployeeId,Name, IsCurrentEmployee FROM Employee ";
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
                    e.Photo,
                    d.Name AS DepartmentName
                FROM committee_member cm
                INNER JOIN Employee e ON cm.EmployeeId = e.EmployeeId
                INNER JOIN Department d ON e.DepartmentId = d.DepartmentId
                WHERE cm.CommitteeId = @committeeId
                AND e.IsCurrentEmployee = 1";
            return connection.Query<CommitteeMemberViewModel>(sql, new {CommitteeId}).ToList();
        }
       

        public void AddCommitteeMember(CommitteeMember member)
        {
            using var conn = _context.CreateConnection();
            string sql = @"INSERT INTO committee_member 
                   (CommitteeId, EmployeeId, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) 
                   VALUES (@CommitteeId, @EmployeeId, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";
            conn.Execute(sql, member);
        }

        public CommitteeMember GetCommitteeMemberById(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = "SELECT * FROM committee_member WHERE CommitteeMemberId = @id";
            return conn.QueryFirstOrDefault<CommitteeMember>(sql, new { id });
        }

        public CommitteeMemberViewModel? GetCommitteeMemberViewModelById(int id)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
        SELECT 
            cm.CommitteeMemberId,
            cm.CommitteeId,
            e.EmployeeId,
            e.Name AS EmployeeName,
            d.Name AS DepartmentName,
            e.Photo
        FROM committee_member cm
        INNER JOIN Employee e ON cm.EmployeeId = e.EmployeeId
        INNER JOIN Department d ON e.DepartmentId = d.DepartmentId
        WHERE cm.CommitteeMemberId = @id";

            return conn.QueryFirstOrDefault<CommitteeMemberViewModel>(sql, new { id });
        }

        public CommitteeMember? GetCommitteeMember(int committeeId, int employeeId)
        {
            using var conn = _context.CreateConnection();
            string sql = "SELECT * FROM committee_member WHERE CommitteeId = @committeeId AND EmployeeId = @employeeId";
            return conn.QueryFirstOrDefault<CommitteeMember>(sql, new { committeeId, employeeId });
        }

        public void UpdateCommitteeMember(CommitteeMember member)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                UPDATE committee_member 
                SET UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE CommitteeId = @CommitteeId AND EmployeeId = @EmployeeId";
            conn.Execute(sql, member);
        }

        public void DeleteCommitteeMember(int committeeMemberId)
        {
            using var conn = _context.CreateConnection();
            string sql = "DELETE FROM committee_member WHERE CommitteeMemberId = @committeeMemberId";
            conn.Execute(sql, new { committeeMemberId });
        }




        public List<int> GetCommitteeIdsByEmployeeId(int employeeId)
        {
            using var db = _context.CreateConnection();
           return db.Query<int>(
            "SELECT CommitteeId FROM committee_member WHERE EmployeeId = @employeeId",
            new { employeeId }
            ).ToList();

        }

        public List<Employee> SearchAvailableEmployees(int committeeId, string term)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                SELECT e.EmployeeId, e.Name
                FROM Employee e
                WHERE e.IsCurrentEmployee = 1
                AND e.Name LIKE @SearchTerm
                AND e.EmployeeId NOT IN (
                SELECT EmployeeId FROM committee_member WHERE CommitteeId = @CommitteeId
                )";
            return conn.Query<Employee>(sql, new { SearchTerm = $"%{term}%", CommitteeId = committeeId }).ToList();
        }

        public Employee GetEmployeeById(int employeeId)
        {
            using var connection = _context.CreateConnection();
            string sql = "SELECT EmployeeId, Name FROM Employee WHERE EmployeeId = @Id";
            return connection.QueryFirstOrDefault<Employee>(sql, new { Id = employeeId });
        }
    }
}
