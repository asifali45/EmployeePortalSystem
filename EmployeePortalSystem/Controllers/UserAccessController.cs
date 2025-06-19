using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class UserAccessController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
