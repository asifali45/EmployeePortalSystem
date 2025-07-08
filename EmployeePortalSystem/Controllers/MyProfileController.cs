using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class MyProfileController : Controller
    {
        private readonly MyProfileRepository _repo;
        private readonly IWebHostEnvironment _env;


        public MyProfileController(MyProfileRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            int? empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "Account");

            var employee = _repo.GetEmployeeDetailsById(empId.Value);
            employee.Committees = _repo.GetCommitteesByEmployeeId(empId.Value);
            employee.Awards = _repo.GetAwardsByEmployeeId(empId.Value);
            employee.Blogs = _repo.GetBlogsByEmployeeId(empId.Value);
            employee.Polls = _repo.GetPollsByEmployeeId(empId.Value);

            if (employee == null)
                return NotFound();
            ViewBag.SelectedOptions = _repo.GetSelectedOptionsByEmployeeId(empId.Value);
            return View(employee);
        }
    }
}
