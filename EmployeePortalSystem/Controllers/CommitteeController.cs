using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Controllers
{
    public class CommitteeController : Controller
    {
        private readonly CommitteeRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public CommitteeController(CommitteeRepository repository,IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
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
            
            var employee=_repository.GetAllEmployees();// Retrieve all employees for the dropdown
            ViewBag.Employees = employee;
            return View();
        }

      

        [HttpPost]
        public IActionResult CreateCommittee(CommitteeViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? logoPath = null;

                if (model.LogoFile != null && model.LogoFile.Length > 0)
                {
                    string? fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                         Path.GetExtension(model.LogoFile.FileName);

                    string imageFullPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        model.LogoFile.CopyTo(stream);
                    }

                    logoPath = fileName;
                    Console.WriteLine("Uploaded file name: " + model.LogoFile.FileName);
                }

                int? UserId = HttpContext.Session.GetInt32("EmployeeId");

                var committee = new Committee
                {
                    Name = model.Name,
                    Type = model.Type,
                    HeadId = model.HeadId,
                    Logo = logoPath, // can be null if no image uploaded
                    Description = model.Description,
                    CreatedBy = UserId,
                    UpdatedBy = UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _repository.CreateCommittee(committee);

                return RedirectToAction("CommitteeDetails");
            }

            // if model validation fails
            TempData["Error"] = "Validation failed. Please check all fields.";
            ViewBag.Employees = _repository.GetAllEmployees();
            return View(model);
        }


    }

}
