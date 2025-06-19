using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
