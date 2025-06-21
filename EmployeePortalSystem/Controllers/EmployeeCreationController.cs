using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using EmployeePortalSystem.ViewModels;


namespace EmployeePortalSystem.Controllers
{
    public class EmployeeCreationController : Controller
    {
        private readonly EmployeeCreationRepository _repo;
        private readonly IWebHostEnvironment _env;

       
        public EmployeeCreationController(IConfiguration config, IWebHostEnvironment env)
        {
            _repo = new EmployeeCreationRepository(config);
            _env = env;
        }

        [HttpGet]
        public IActionResult Employee()
        {
            var departments = _repo.GetDepartments(); 
            var roles = _repo.GetRoles();             

            var vm = new EmployeeCreationViewModel
            {
                Employee = new Employee(),
                Departments = departments,
                Roles = roles
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult Employee(Employee employee, IFormFile Photo, IFormCollection form)
        {
            string selectedRole = form["Role"]; 
            employee.IsAdmin = selectedRole == "Admin";
            if (Photo != null && Photo.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + "_" + Path.GetFileName(Photo.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(fs);
                }

                employee.Photo = "/uploads/" + fileName;
            }

            employee.CreatedAt = DateTime.Now;
            employee.UpdatedAt = DateTime.Now;

            _repo.AddEmployee(employee); // Use Dapper to insert employee with RoleId, DepartmentId, etc.

            TempData["Message"] = "Employee added successfully!";
            return RedirectToAction("Employee");
        }

    }
}