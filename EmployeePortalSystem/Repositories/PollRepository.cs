using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Repositories
{
    public class PollRepository
    {
        private readonly AppDbContext _context;

        public PollRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all polls with creator name
        public List<PollViewModel> GetAll()
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                SELECT p.PollId, p.Question, p.Option1, p.Option2, p.Option3, p.Option4,
                       p.CreatedAt, e.Name AS CreatedByName
                FROM polls p
                JOIN employee e ON p.CreatedBy = e.EmployeeId
                ORDER BY p.CreatedAt DESC";

            return conn.Query<PollViewModel>(sql).ToList();
        }

        // Get a poll by ID
        public Poll GetById(int id)
        {
            using var conn = _context.CreateConnection();
            return conn.QuerySingleOrDefault<Poll>("SELECT * FROM polls WHERE PollId = @Id", new { Id = id });
        }

        // Add a new poll
        public void Add(Poll poll)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                INSERT INTO polls (Question, Option1, Option2, Option3, Option4, CreatedBy, CreatedAt)
                VALUES (@Question, @Option1, @Option2, @Option3, @Option4, @CreatedBy, @CreatedAt)";
            conn.Execute(sql, poll);
        }

        // Update an existing poll
        public void Update(Poll poll)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                UPDATE polls SET
                    Question = @Question,
                    Option1 = @Option1,
                    Option2 = @Option2,
                    Option3 = @Option3,
                    Option4 = @Option4
                WHERE PollId = @PollId";
            conn.Execute(sql, poll);
        }

        // Delete a poll
        public void Delete(int id)
        {
            using var conn = _context.CreateConnection();
            conn.Execute("DELETE FROM polls WHERE PollId = @Id", new { Id = id });
        }

        // Submit a poll response
        public void SubmitResponse(PollResponse response)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                INSERT INTO poll_response (PollId, EmployeeId, SelectedOption, CreatedAt)
                VALUES (@PollId, @EmployeeId, @SelectedOption, @CreatedAt)";
            conn.Execute(sql, response);
        }

        // Get result summary (votes per option) for a poll
        public Dictionary<string, int> GetResults(int pollId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
                SELECT SelectedOption, COUNT(*) AS Count
                FROM poll_response
                WHERE PollId = @PollId
                GROUP BY SelectedOption";

            var rows = conn.Query(sql, new { PollId = pollId });

            var result = new Dictionary<string, int>();
            foreach (var row in rows)
            {
                result[(string)row.SelectedOption] = (int)row.Count;
            }

            return result;
        }

        // Check if employee already voted in a poll
        public bool HasVoted(int pollId, int employeeId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"SELECT COUNT(*) FROM poll_response WHERE PollId = @PollId AND EmployeeId = @EmployeeId";
            int count = conn.ExecuteScalar<int>(sql, new { PollId = pollId, EmployeeId = employeeId });
            return count > 0;
        }


        public List<PollResponseViewModel> GetResponsesWithEmployee(int pollId)
        {
            using var conn = _context.CreateConnection();
            string sql = @"
    SELECT pr.ResponseId AS PollResponseId, pr.PollId, pr.EmployeeId, e.Name AS EmployeeName,
           pr.SelectedOption, pr.CreatedAt
    FROM poll_response pr
    JOIN employee e ON pr.EmployeeId = e.EmployeeId
    WHERE pr.PollId = @PollId
    ORDER BY pr.CreatedAt";

            return conn.Query<PollResponseViewModel>(sql, new { PollId = pollId }).ToList();
        }

    }
}
