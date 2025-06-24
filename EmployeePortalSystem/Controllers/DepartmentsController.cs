using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var departments = _repo.GetAllWithDetails();
            return View(departments); // View expects List<DepartmentViewModel>
        }

        public IActionResult Details(int id)
        {
            var dept = _repo.GetById(id);
            return View(dept);
        }
        public IActionResult Create()
        {
            var departments = _repo.GetAll()
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name
                })
                .ToList();

            ViewBag.ParentDepartments = departments;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Department model)
        {
            if (!ModelState.IsValid)
            {
                // Reload for validation errors
                var depts = _repo.GetAll()
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    })
                    .ToList();
                ViewBag.ParentDepartments = depts;
                return View(model);
            }

            model.CreatedBy = 1;
            _repo.Add(model);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var dept = _repo.GetById(id);
            if (dept == null) return NotFound();

            // Get head name using HeadId
            string headName = _repo.GetHeadNameById(dept.HeadId);

            // Get departments for dropdown excluding the current department
            var departments = _repo.GetAll()
                .Where(d => d.DepartmentId != id)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.Name,
                    Selected = (d.DepartmentId == dept.ParentDepartmentId)
                }).ToList();

            ViewBag.HeadName = headName;
            ViewBag.ParentDepartments = departments;

            return View(dept);
        }



        [HttpPost]
        public IActionResult Edit(Department dept)
        {
            dept.UpdatedBy = 1;
            _repo.Update(dept);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
