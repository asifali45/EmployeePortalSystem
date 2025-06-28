using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Models;

namespace EmployeePortalSystem.Controllers
{
    public class CommitteeController : Controller
    {
        private readonly CommitteeRepository _repository;

        public CommitteeController(CommitteeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult CommitteeDetails()
        {
            // Here you would typically retrieve committee details from the repository
            var committeeList = _repository.GetAllCommittees();


            return View("CommitteeDetails", committeeList);
        }

        [HttpGet]

        public IActionResult CreateCommittee()
        {
            return View();
        }

        [HttpPost]

        public IActionResult CreateCommittee(Committee committee)
        {
            if(ModelState.IsValid)
            {
                //get the logged-in user ID from session
                int? UserId = HttpContext.Session.GetInt32("EmployeeId");

                committee.CreatedAt = DateTime.Now;
                committee.UpdatedAt = DateTime.Now;
                committee.CreatedBy = UserId; 
                committee.UpdatedBy = UserId;
                
                
                _repository.CreateCommittee(committee);

                return RedirectToAction("CommitteeDetails");
            }
            return View(committee);
        }
        
        
    }

}
