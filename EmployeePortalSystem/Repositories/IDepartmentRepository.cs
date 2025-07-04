using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;

public interface IDepartmentRepository
{
    IEnumerable<Department> GetAll();
    List<DepartmentViewModel> GetAllWithDetails();
    Department GetById(int id);
    void Add(Department dept);
    void Update(Department dept);
    void Delete(int id);
    IEnumerable<Employee> SearchEmployeesByName(string namePart);
    string? GetHeadNameById(int? headId);

    IEnumerable<Employee> GetAllEmployees();
}
