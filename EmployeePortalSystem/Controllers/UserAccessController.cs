using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;
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
        public IActionResult Login(LoginViewModel loginVM)
        {
            
            if (!ModelState.IsValid)
            {

                return View(loginVM);
            }


            var employee = _repository.EmployeeLogin(loginVM.Email, loginVM.Password);

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
                return View(loginVM);
            }
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

            int empid=Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
         


            var model =_repository.GetCardCounts(empid);

            return View(model);
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
