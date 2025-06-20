using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class EmployeeCreationController : Controller
    {
        public IActionResult Employee()
        {
            return View();
        }
    }
}
