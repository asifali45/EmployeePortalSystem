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
        private readonly AnnouncementRepository _announcementRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly CommitteeRepository _committeeRepository;
        private readonly AwardRepository _awardRepository;
        private readonly PollRepository _pollsRepository;

        public UserAccessController(UserAccessRepository repository,
            BlogsRepository blogsRepository,AnnouncementRepository announcementRepository,
            EmployeeRepository employeeRepository, CommitteeRepository committeeRepository, AwardRepository awardRepository, PollRepository pollsRepository)
        {
            _repository = repository;
            _blogsRepository = blogsRepository;
            _announcementRepository = announcementRepository;
            _employeeRepository = employeeRepository;
            _committeeRepository = committeeRepository;
            _awardRepository = awardRepository;
            _pollsRepository = pollsRepository;
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


            var latestAnnouncements = _announcementRepository
               .GetLatestAnnouncements(2);

            var latestawards = _awardRepository.GetAwardsForDashboard(2);

            var latestpolls = _pollsRepository.GetAll(2);

            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in latestpolls)
            {
                results[poll.PollId] = _pollsRepository.GetResults(poll.PollId);
            }
            ViewBag.Results = results;



            var model = new DashboardCardViewModel
            {
                LatestBlogs = latestblogs,
                LatestAnnouncements = latestAnnouncements.ToList(),
                LatestAwards = latestawards.ToList(),
                LatestPolls=latestpolls.ToList()


            };
            return View(model);
        }

        [HttpGet]
        public IActionResult DashboardEmployee()
        {
            HttpContext.Session.SetString("CurrentDashboard", "Employee");

            int empid=Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            //Card counts
            var cardcounts =_repository.GetCardCounts(empid);

            //fetch blogs
            var latestblogs=_blogsRepository.GetLatestBlogsForDashboard(2);

            // Get department & committee IDs for Announcement Visibility
            var deptId = _employeeRepository.GetDepartmentIdByEmployeeId(empid);
            var committeeIds = _committeeRepository.GetCommitteeIdsByEmployeeId(empid);
            //  Fetch  announcements
            var latestAnnouncements = _announcementRepository
                .GetLatestVisibleAnnouncementsForEmployee(deptId, committeeIds, 2);

            // Fetch Awards
            var latestawards = _awardRepository.GetAwardsForDashboard(2);

            //Fetch Polls
            var latestpolls = _pollsRepository.GetAll(2);

            var selectedOptions = _pollsRepository.GetSelectedOptionsForEmployee(empid); // get past votes

            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in latestpolls)
            {
                results[poll.PollId] = _pollsRepository.GetResults(poll.PollId);
            }

            ViewBag.SelectedOptions = selectedOptions;
            ViewBag.Results = results;

            //  Prepare chart data
            var contributionData = new Dictionary<string, int>
            {
                { "Blogs", cardcounts.BlogsWritten },
                { "Polls", cardcounts.PollsVoted },
                { "Awards", cardcounts.TotalAwards }
            };

            var model = new DashboardCardViewModel
            {
                TotalAwards = cardcounts.TotalAwards,
                BlogsWritten = cardcounts.BlogsWritten,
                PollsVoted = cardcounts.PollsVoted,
                LatestBlogs = latestblogs,
                LatestAnnouncements = latestAnnouncements.ToList(),
                LatestAwards= latestawards.ToList(),
                LatestPolls=latestpolls.ToList(),
                ContributionChartData = contributionData
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

        [HttpGet]
        public IActionResult Signup()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public IActionResult Signup(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool updated = _repository.UpdatePasswordIfEmailExists(model.Email, model.Password);

            if (!updated)
            {
                ViewBag.ErrorMessage = "Email not found.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Password reset successful. You can now login.";
            return View(new SignUpViewModel());
        }


    }
}
