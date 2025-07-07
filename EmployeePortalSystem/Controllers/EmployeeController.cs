using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using EmployeePortalSystem.ViewModels;


namespace EmployeePortalSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _repo;
        private readonly IWebHostEnvironment _env;

       
        public EmployeeController(EmployeeRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }
        [HttpGet]
        public IActionResult EmployeeDetails()
        {
            var employeeList = _repo.GetAllEmployeeDetails();
            return View("EmployeeDetails", employeeList); 
        }
        [HttpGet]
        public IActionResult EmployeeDetailsForEmployee()
        {
            var employeeList = _repo.GetAllEmployeeDetails();
            return View("EmployeeDetailsForEmployee", employeeList);
        }

        [HttpGet]
        public IActionResult EmployeeInsertion()
        {
            var departments = _repo.GetDepartments(); 
            var roles = _repo.GetRoles();             

            var vm = new EmployeeInsertionViewModel
            {
                Employee = new Employee(),
                Departments = departments,
                Roles = roles
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var emp = _repo.GetEmployeeById(id);
            if (emp == null)
                return NotFound();

            var departments = _repo.GetDepartments();
            var roles = _repo.GetRoles();

            var vm = new EmployeeInsertionViewModel
            {
                Employee = emp,
                Departments = departments,
                Roles = roles
            };

            return View("EmployeeInsertion", vm); 
        }

        [HttpGet]
        public IActionResult EmployeeDelete(int id)
        {
            var employee = _repo.GetEmployeeById(id);
            if (employee == null)
                return NotFound();

            return View("EmployeeDelete", employee);
        }

        [HttpPost]
        public IActionResult EmployeeDeleteConfirmed(int id)
        {
            int? loggedInAdminId = HttpContext.Session.GetInt32("EmployeeId");
            if (id == loggedInAdminId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("EmployeeDetails");
            }

            _repo.DeleteEmployee(id);
            TempData["Messages"] = "Employee deleted successfully!";
            return RedirectToAction("EmployeeDetails");
        }


        [HttpPost]
        public IActionResult EmployeeInsertion(Employee employee, IFormFile? Photo, IFormCollection form)
        {

            string selectedRole = form["Role"]; 
            employee.IsAdmin = selectedRole == "Admin";

            

            if (!ModelState.IsValid)
            {

                var vm = new EmployeeInsertionViewModel
                {
                    Employee = employee,
                    Departments = _repo.GetDepartments(),
                    Roles = _repo.GetRoles()
                };

                return View("EmployeeInsertion", vm);
            }
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

                //employee.Photo = "/uploads/" + fileName;
                employee.Photo = fileName;
            }
            else if (employee.EmployeeId > 0)
            {
                
                var existing = _repo.GetEmployeeById(employee.EmployeeId);
                employee.Photo = existing?.Photo;
            }
            else
            {
               
                employee.Photo = null;
            }

            int? userId = HttpContext.Session.GetInt32("EmployeeId");

            if (employee.EmployeeId > 0)
            {
               
                employee.UpdatedAt = DateTime.Now;
                employee.UpdatedBy = userId;

                _repo.UpdateEmployee(employee);

                TempData["Message1"] = "Employee updated successfully!";
            }
            else
            {
                employee.CreatedAt = DateTime.Now;
                employee.UpdatedAt = DateTime.Now;
                employee.CreatedBy = userId;

                _repo.AddEmployee(employee);

                TempData["Message1"] = "Employee added successfully!";
            }
            return RedirectToAction("EmployeeInsertion");
        }

    }
}