using EmployeePortalSystem.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace EmployeePortalSystem.Repositories
{
    public class AnnouncementRepository:IAnnouncementRepository
    {
        private readonly string _connectionString;

        public AnnouncementRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public IEnumerable<Announcement> GetAll()
        {
            using var db = Connection;
            return db.Query<Announcement>("SELECT * FROM announcement ORDER BY DisplayOrder ASC");
        }


        public Announcement GetById(int id)
        {
            using var db = Connection;
            return db.QueryFirstOrDefault<Announcement>("SELECT * FROM announcement WHERE AnnouncementId = @id", new { id });
        }

        public void Add(Announcement announcement)
        {
            using var db = Connection;

            string sql = @"
        INSERT INTO announcement
        (Title, Message, PostDate, VisibleTo, DisplayOrder, IsEvent, EventDate, EventTime, Location, CreatedBy)
        VALUES
        (@Title, @Message, @PostDate, @VisibleTo, @DisplayOrder, @IsEvent, @EventDate, @EventTime, @Location, @CreatedBy)";

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
            using var db = Connection;
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
            using var db = Connection;
            db.Execute("DELETE FROM announcement WHERE AnnouncementId = @id", new { id });
        }
    }
}
