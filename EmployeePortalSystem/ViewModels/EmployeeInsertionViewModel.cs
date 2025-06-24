using EmployeePortalSystem.Models;
using System.Collections.Generic;

namespace EmployeePortalSystem.ViewModels
{
    public class EmployeeInsertionViewModel
    {
        public Employee Employee { get; set; }

        public List<Department> Departments { get; set; }

        public List<Role> Roles { get; set; }
    }
}
