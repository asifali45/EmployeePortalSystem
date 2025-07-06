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
        private readonly AppDbContext _context;

        public AwardRepository(AppDbContext context)
        {
            _context = context;

        }
        public async Task<int> GetEmployeeIdByNameAsync(string name)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT EmployeeId FROM Employee WHERE Name = @Name LIMIT 1";
                var result = await connection.QueryFirstOrDefaultAsync<int?>(query, new { Name = name });
                return result ?? 0; 
            }
        }
        public async Task<string> GetEmployeeNameByIdAsync(int employeeId)
        {
            var query = "SELECT Name FROM Employee WHERE EmployeeId = @EmployeeId LIMIT 1";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<string>(query, new { EmployeeId = employeeId });
            return result ?? string.Empty;
        }
        public async Task<int> CreateAsync(Award award)
        {
            var query = @"INSERT INTO Awards (Type, EventDate, RecipientId, GivenBy, Description, DisplayOrder, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) 
                  VALUES (@Type, @EventDate, @RecipientId, @GivenBy, @Description, @DisplayOrder, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                  SELECT LAST_INSERT_ID();";

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.ExecuteScalarAsync<int>(query, award);
                return id;
            }
        }


        public async Task<IEnumerable<Award>> GetAllAsync()
        {
            var query = @"SELECT  a.AwardId, a.Type, a.EventDate, 
                    a.RecipientId, e.Name as RecipientName,
                    e.Photo as RecipientPhoto,
                    a.GivenBy, a.Description, a.DisplayOrder
                  FROM Awards a
                  JOIN Employee e ON a.RecipientId = e.EmployeeId
                  ORDER BY a.DisplayOrder";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Award>(query);
        }
        public async Task<List<string>> SearchEmployeeNamesAsync(string term)
        {
            var query = "SELECT Name FROM Employee WHERE Name LIKE @SearchTerm LIMIT 10";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<string>(query, new { SearchTerm = "%" + term + "%" });
            return result.ToList();
        }


        public async Task<Award> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Awards WHERE AwardId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Award>(query, new { Id = id });
        }
        // Get Award By ID
        public async Task<Award> GetAwardByIdAsync(int id)
        {
            var query = "SELECT * FROM Awards WHERE AwardId = @AwardId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Award>(query, new { AwardId = id });
            }
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

        // Delete Award By ID
        public async Task<int> DeleteAwardAsync(int id)
        {
            var query = "DELETE FROM Awards WHERE AwardId = @AwardId";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(query, new { AwardId = id });
            }
        }
    }
}
