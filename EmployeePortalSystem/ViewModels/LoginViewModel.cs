using System.ComponentModel.DataAnnotations;

namespace EmployeePortalSystem.ViewModels
{
    public class LoginViewModel
    {

        public int EmployeeId { get; set; }

        public string Name { get; set; }
        public string RoleName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
