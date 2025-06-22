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
                
                
                return RedirectToAction("DashboardAdmin","UserAccess");
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
            return View();
        }

        [HttpGet]
        public IActionResult DashboardEmployee()
        {
            return View();
        }



    }
}
