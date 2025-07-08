using EmployeePortalSystem.Models;
using EmployeePortalSystem.Context;
using Dapper;
using System.Data;

namespace EmployeePortalSystem.Repositories
{
    public class AnnouncementRepository
    {
        private readonly AppDbContext _dbContext;

        public AnnouncementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Announcement> GetAll()
        {
            using var db = _dbContext.CreateConnection();
            return db.Query<Announcement>("SELECT * FROM announcement ORDER BY DisplayOrder ASC");
        }

        public Announcement GetById(int id)
        {
            using var db = _dbContext.CreateConnection();
            return db.QueryFirstOrDefault<Announcement>("SELECT * FROM announcement WHERE AnnouncementId = @id", new { id });
        }

        public void Add(Announcement announcement)
        {
            using var db = _dbContext.CreateConnection();

            string sql = @"
                INSERT INTO announcement
                (Title, Message, PostDate, VisibleTo, VisibleToDepartmentId, VisibleToCommitteeId, DisplayOrder, IsEvent, EventDate, EventTime, Location, CreatedBy)
                VALUES
                (@Title, @Message, @PostDate, @VisibleTo, @VisibleToDepartmentId, @VisibleToCommitteeId, @DisplayOrder, @IsEvent, @EventDate, @EventTime, @Location, @CreatedBy)";


            var parameters = new
            {
                announcement.Title,
                announcement.Message,
                PostDate = DateTime.Now,
                announcement.VisibleTo,
                announcement.DisplayOrder,
                IsEvent = (announcement.IsEvent ?? false) ? 1 : 0,
                EventDate = (announcement.IsEvent ?? false) ? announcement.EventDate : null,
                EventTime = (announcement.IsEvent ?? false) ? announcement.EventTime : null,
                announcement.Location,
                announcement.CreatedBy
            };

            var rows = db.Execute(sql, parameters);
            Console.WriteLine($"✅ Rows inserted: {rows}");
        }

        public void Update(Announcement announcement)
        {
            using var db = _dbContext.CreateConnection();

            string sql = @"
                UPDATE announcement SET
                    Title = @Title,
                    Message = @Message,
                    PostDate = @PostDate,
                    VisibleTo = @VisibleTo,
                    DisplayOrder = @DisplayOrder,
                    IsEvent = @IsEvent,
                    EventDate = @EventDate,
                    EventTime = @EventTime,
                    Location = @Location,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = NOW()
                WHERE AnnouncementId = @AnnouncementId";

            db.Execute(sql, announcement);
        }

        public void Delete(int id)
        {
            using var db = _dbContext.CreateConnection();
            db.Execute("DELETE FROM announcement WHERE AnnouncementId = @id", new { id });
        }



        public IEnumerable<Announcement> GetVisibleToEmployee(string visibleTo = "All")
        {
            using var db = _dbContext.CreateConnection();

            string sql = @"
        SELECT * FROM announcement
        WHERE VisibleTo = @visibleTo OR VisibleTo = 'All'
        ORDER BY DisplayOrder ASC";

            return db.Query<Announcement>(sql, new { visibleTo });
        }

    }
}
