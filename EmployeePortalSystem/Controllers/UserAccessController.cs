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

                return RedirectToAction("Dashboard","UserAccess");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                
            }
            return View();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }



    }
}
