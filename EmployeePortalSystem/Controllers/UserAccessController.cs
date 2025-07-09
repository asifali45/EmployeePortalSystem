using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace EmployeePortalSystem.Controllers
{
    public class UserAccessController : Controller
    {
       private readonly UserAccessRepository _repository;
        private readonly BlogsRepository _blogsRepository;

        public UserAccessController(UserAccessRepository repository,BlogsRepository blogsRepository)
        {
            _repository = repository;
            _blogsRepository = blogsRepository;
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


                HttpContext.Session.SetString("Role", employee.IsAdmin ? "admin" : "employee");

                // ✅ Set "CurrentDashboard" for cancel redirection in Delete.cshtml
                HttpContext.Session.SetString("CurrentDashboard", employee.IsAdmin ? "Admin" : "Employee");

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
            var latestblogs = _blogsRepository.GetLatestBlogsForDashboard(2);
            var model = new DashboardCardViewModel
            {
                LatestBlogs = latestblogs
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult DashboardEmployee()
        {
            HttpContext.Session.SetString("CurrentDashboard", "Employee");

            int empid=Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
         


            var cardcounts =_repository.GetCardCounts(empid);

            var latestblogs=_blogsRepository.GetLatestBlogsForDashboard(2);

            var model = new DashboardCardViewModel
            {
                TotalAwards = cardcounts.TotalAwards,
                BlogsWritten = cardcounts.BlogsWritten,
                PollsVoted = cardcounts.PollsVoted,
                LatestBlogs = latestblogs
            };

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
