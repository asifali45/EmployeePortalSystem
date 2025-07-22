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
        public IActionResult Profile(string? activeTab)
        {
            int? empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "Account");

            var employee = _repo.GetEmployeeDetailsById(empId.Value);
            employee.Committees = _repo.GetCommitteesByEmployeeId(empId.Value);
            employee.Awards = _repo.GetAwardsByEmployeeId(empId.Value);
            employee.Blogs = _repo.GetBlogsByEmployeeId(empId.Value);
            employee.Polls = _repo.GetPollsByEmployeeId(empId.Value);

            //if (employee == null)
            //    return NotFound();
            //ViewBag.SelectedOptions = _repo.GetSelectedOptionsByEmployeeId(empId.Value);
            //return View(employee);
            var selected = _repo.GetSelectedOptionsByEmployee(empId.Value);
            ViewBag.SelectedOptions = selected;

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? "awards";
            return View(employee);
        }
        [HttpGet]
        public IActionResult EditPhoto()
        {
            int? empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "UserAccess");

            var employee = _repo.GetEmployeeById(empId.Value);
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EditPhoto(IFormFile PhotoFile)
        {
            int? empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "UserAccess");

            var employee = _repo.GetEmployeeById(empId.Value);

            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(PhotoFile.FileName);
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }

                employee.Photo = fileName;
                _repo.UpdateEmployeePhoto(empId.Value, fileName); // Add this method in repo
                //TempData["Message1"] = "Photo updated successfully!";
            }

            return RedirectToAction("Profile");
        }

    }
}
