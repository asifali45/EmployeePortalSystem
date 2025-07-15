using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class RolesController : Controller
    {
        private readonly RolesRepository _repo;
       


        public RolesController(RolesRepository repo)
        {
            _repo = repo;
        }
        public IActionResult RolesList()
        {
            var RolesList=_repo.GetAllRoles();
            return View(RolesList);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View(new Role());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRole(Role role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            int? userId = HttpContext.Session.GetInt32("EmployeeId");
            role.CreatedBy = userId ?? 0;
            role.CreatedAt = DateTime.Now;

            _repo.AddRole(role);
            TempData["Message8"] = "Role added successfully!";
            return RedirectToAction("RolesList");
        }
    }
}
