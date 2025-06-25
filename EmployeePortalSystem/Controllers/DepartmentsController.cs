using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;
using System.Linq;

namespace EmployeePortalSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentsController(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var departments = _repo.GetAllWithDetails(); // Returns List<DepartmentViewModel>
            return View(departments);
        }

        public IActionResult Details(int id)
        {
            var dept = _repo.GetById(id);
            return View(dept);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.ParentDepartments = GetParentDepartmentsDropdown();
            return View();
        }

        // POST: Create
        [HttpPost]
        public IActionResult Create(Department model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentDepartments = GetParentDepartmentsDropdown();
                return View(model);
            }

            Console.WriteLine("Received HeadId: " + model.HeadId);

            model.CreatedBy = 1;
            _repo.Add(model);
            return RedirectToAction("Index");
        }

        // GET: Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var dept = _repo.GetById(id);
            if (dept == null) return NotFound();

            string headName = _repo.GetHeadNameById(dept.HeadId);
            ViewBag.HeadName = headName;

            ViewBag.ParentDepartments = _repo.GetAll()
                .Where(d => d.DepartmentId != id)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name,
                    Selected = d.DepartmentId == dept.ParentDepartmentId
                }).ToList();

            return View(dept);
        }

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


        // POST: Edit
        [HttpPost]
        public IActionResult Edit(Department dept)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.HeadName = _repo.GetHeadNameById(dept.HeadId);
                ViewBag.ParentDepartments = GetParentDepartmentsDropdown(dept.DepartmentId);
                return View(dept);
            }

            dept.UpdatedBy = 1;
            _repo.Update(dept);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var dept = _repo.GetById(id);
            if (dept == null)
            {
                return NotFound();
            }

            return View(dept); // Shows confirmation view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }

        // 🔁 Helper method to generate dropdown list
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
    }
}
