using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class AwardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
