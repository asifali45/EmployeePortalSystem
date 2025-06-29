using EmployeePortalSystem.Models;

namespace EmployeePortalSystem.Repositories
{
    public interface IAnnouncementRepository
    {
        IEnumerable<Announcement> GetAll();
        Announcement GetById(int id);
        void Add(Announcement announcement);
        void Update(Announcement announcement);
        void Delete(int id);
    }
}
