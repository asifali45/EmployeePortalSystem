using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class UserAccessController : Controller
    {
       private readonly UserAccessRepository _repository;

        public UserAccessController(UserAccessRepository repository)
        {
            _repository = repository;
        }
       


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Login(string email,string password)
        {
            var employee = _repository.EmployeeLogin(email, password);

            if (employee != null)
            {
                HttpContext.Session.SetInt32("EmployeeId", employee.EmployeeId);
                HttpContext.Session.SetString("EmployeeName", employee.Name);
                HttpContext.Session.SetString("Designation", employee.RoleName);
                HttpContext.Session.SetString("IsAdmin", employee.IsAdmin.ToString());

                return RedirectToAction("DashboardEmployee", "UserAccess");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                
            }
            return View();
        }

        [HttpGet]
        public IActionResult DashboardAdmin()
        {
            HttpContext.Session.SetString("CurrentDashboard", "Admin");
            return View();
        }

        [HttpGet]
        public IActionResult DashboardEmployee()
        {
            HttpContext.Session.SetString("CurrentDashboard", "Employee");
            return View();
        }

        [HttpPost]
        public IActionResult SwitchToAdmin()
        {
            return RedirectToAction("DashboardAdmin", "UserAccess");
        }

        [HttpPost]
        public IActionResult SwitchToEmployee()
        {
            return RedirectToAction("DashboardEmployee", "UserAccess");
        }




    }
}
