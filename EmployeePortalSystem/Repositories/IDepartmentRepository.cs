using EmployeePortalSystem.Models;
using System.Collections.Generic;


namespace EmployeePortalSystem.Repositories
{
    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetAll();              
        Department GetById(int id);                    
        void Add(Department dept);                    
        void Update(Department dept);                  
        void Delete(int id);
    }
}