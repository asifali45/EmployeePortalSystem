using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using System.Linq;
using EmployeePortalSystem.Context;

namespace EmployeePortalSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly DepartmentRepository _repo;

        public DepartmentsController(DepartmentRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var departments = _repo.GetAllWithDetails();
            return View(departments);
        }

        public IActionResult EmployeeDepartmentDetails()
        {
            var departments = _repo.GetAllWithDetails();
            return View(departments);
        }

        public IActionResult DepartmentMembers(int id)
        {
            var department = _repo.GetById(id);
            if (department == null)
                return NotFound();

            ViewBag.DepartmentName = department.Name;
            var employees = _repo.GetEmployeesByDepartmentId(id);
            return View("DepartmentMembers", employees);
        }


        public IActionResult Create()
        {
            ViewBag.ParentDepartments = GetParentDepartmentsDropdown();
            return View(new Department());
        }

        [HttpPost]
        public IActionResult Create(Department model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentDepartments = GetParentDepartmentsDropdown();
                return View(model);
            }

            model.CreatedBy = 1;
            _repo.Add(model);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var dept = _repo.GetById(id);
            if (dept == null) return NotFound();

            ViewBag.ParentDepartments = GetParentDepartmentsDropdown(dept.DepartmentId);
            ViewBag.HeadName = _repo.GetHeadNameById(dept.HeadId);
            return View(dept);
        }

        [HttpPost]
        public IActionResult Edit(Department dept)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentDepartments = GetParentDepartmentsDropdown(dept.DepartmentId);
                ViewBag.HeadName = _repo.GetHeadNameById(dept.HeadId);
                return View(dept);
            }

            dept.UpdatedBy = 1;
            _repo.Update(dept);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var dept = _repo.GetById(id);
            if (dept == null)
            {
                return NotFound();
            }

            return View(dept);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            TempData["Message"] = "Department and assigned employees deleted successfully.";
            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetParentDepartmentsDropdown(int? excludeId = null)
        {
            return _repo.GetAll()
                .Where(d => excludeId == null || d.DepartmentId != excludeId)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                }).ToList();
        }

        //  Supports JavaScript-based HeadName search
        [HttpGet]
        public JsonResult SearchEmployees(string term)
        {
            var employees = _repo.SearchEmployeesByName(term)
                                 .Select(e => new {
                                     employeeId = e.EmployeeId,
                                     name = e.Name
                                 }).ToList();

            return Json(employees);
        }
    }
}
