using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;

namespace EmployeePortalSystem.Repositories
{
    public class AwardRepository
    {
        private readonly AwardContext _context;

        public AwardRepository(AwardContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Award>> GetAllAsync()
        {
            var query = @"SELECT  a.AwardId, a.Type, a.EventDate, 
                    a.RecipientId, e.Name as RecipientName, 
                    a.GivenBy, a.Description, a.DisplayOrder
                  FROM Awards a
                  JOIN Employee e ON a.RecipientId = e.EmployeeId
                  ORDER BY a.DisplayOrder";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Award>(query);
        }

        public async Task<Award> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Awards WHERE AwardId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Award>(query, new { Id = id });
        }

        public async Task<int> CreateAsync(Award award)
        {
            Console.WriteLine($"Inserting Award: Type={award.Type}, EventDate={award.EventDate}, RecipientId={award.RecipientId}, CreatedBy={award.CreatedBy}, UpdatedBy={award.UpdatedBy}");
 

            var query = @"INSERT INTO Awards 
                (Type, EventDate, RecipientId, GivenBy, Description, DisplayOrder, CreatedBy, UpdatedBy)
                 VALUES 
                (@Type, @EventDate, @RecipientId, @GivenBy, @Description, @DisplayOrder, @CreatedBy, @UpdatedBy);
                 SELECT LAST_INSERT_ID();";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, award);
        }


        public async Task<bool> UpdateAsync(Award award)
        {
            var query = @"UPDATE Awards SET 
                          Type = @Type, 
                          EventDate = @EventDate, 
                          RecipientId = @RecipientId, 
                          GivenBy = @GivenBy, 
                          Description = @Description, 
                          DisplayOrder = @DisplayOrder,
                          UpdatedBy = @UpdatedBy
                          WHERE AwardId = @AwardId";

            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, award);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Awards WHERE AwardId = @Id";
            using var connection = _context.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
