using EmployeePortalSystem.Models;
using System.Collections.Generic;

namespace EmployeePortalSystem.ViewModels
{
    public class EmployeeCreationViewModel
    {
        public Employee Employee { get; set; }

        public List<Department> Departments { get; set; }

        public List<Role> Roles { get; set; }
    }
}
