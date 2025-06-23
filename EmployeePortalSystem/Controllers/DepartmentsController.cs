using EmployeePortalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Repositories;


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

            var departments = _repo.GetAll();
            return View(departments);
        }

        public IActionResult Details(int id)
        {
            var dept = _repo.GetById(id);
            return View(dept);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Department dept)
        {
            dept.CreatedBy = 1;
            _repo.Add(dept);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var dept = _repo.GetById(id);
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
