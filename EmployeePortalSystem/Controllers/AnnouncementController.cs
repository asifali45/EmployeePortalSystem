using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeePortalSystem.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly AnnouncementRepository _announcementrepo;
        private readonly DepartmentRepository _repo;
        private readonly CommitteeRepository _repository;

        public AnnouncementController(
            AnnouncementRepository announcementRepo,
            DepartmentRepository departmentRepo,
            CommitteeRepository committeeRepo)
        {
            _announcementrepo = announcementRepo;
            _repo = departmentRepo;
            _repository = committeeRepo;
        }


        public IActionResult Index()
        {
            var announcements = _announcementrepo.GetAll();                     
            return View(announcements);
     
        }


        [HttpGet]
        public IActionResult Create()

        {
            ViewBag.Departments = new SelectList(_repo.GetAll(), "DepartmentId", "Name");
            ViewBag.Committees = new SelectList(_repository.GetAllCommittees(), "CommitteeId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Announcement announcement)
        {
            // Remove AnnouncementId if posted accidentally
            ModelState.Remove("AnnouncementId");

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = new SelectList(_repo.GetAll(), "DepartmentId", "Name");
                ViewBag.Committees = new SelectList(_repository.GetAllCommittees(), "CommitteeId", "Name");
                return View(announcement);

            }

            // Set server-side fields
            announcement.PostDate = DateTime.Now;
            announcement.CreatedBy = 1;

            // ✅ Handle image upload
            if (announcement.ImageFile != null && announcement.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/announcements");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(announcement.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    announcement.ImageFile.CopyTo(stream);
                }

                announcement.ImagePath = "/uploads/announcements/" + uniqueFileName;

            }

            _announcementrepo.Add(announcement);

            TempData["Message5"] = "Announcement created successfully!";
            return RedirectToAction("Index");
        }



        public IActionResult Edit(int id)
        {
            var announcement = _announcementrepo.GetById(id);
            if (announcement == null) return NotFound();

            ViewBag.VisibleToOptions = GetVisibleToOptions();
            ViewBag.Departments = new SelectList(_repo.GetAll(), "DepartmentId", "Name");
            ViewBag.Committees = new SelectList(_repository.GetAllCommittees(), "CommitteeId", "Name");

            return View(announcement);
        }


        [HttpPost]
        public IActionResult Edit(Announcement model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.VisibleToOptions = GetVisibleToOptions();
                return View(model);
            }

            // Get existing record
            var existing = _announcementrepo.GetById(model.AnnouncementId);
            if (existing == null) return NotFound();

            model.PostDate = existing.PostDate; // Preserve original PostDate
            model.UpdatedBy = 1;
            model.UpdatedAt = DateTime.Now;


            // ✅ If a new image is uploaded, save it
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/announcements");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                model.ImagePath = "/uploads/announcements/" + uniqueFileName;
            }
            else
            {
                // ✅ Keep the old image if no new one is uploaded
                model.ImagePath = existing.ImagePath;
            }


                _announcementrepo.Update(model);
            TempData["Message5"] = "Announcement updated successfully!";
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var announcement = _announcementrepo.GetById(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _announcementrepo.Delete(id);
            TempData["Message5"] = "Announcement deleted successfully!";
            return RedirectToAction("Index");
        }

        private static SelectList GetVisibleToOptions()
        {
            return new SelectList(new[]
            {
                new { Value = "All", Text = "All" },
                new { Value = "Department", Text = "Department" },
                new { Value = "Committee", Text = "Committee" }
            }, "Value", "Text");
        }

        public IActionResult EmployeeAnnouncement()
        {
            // Get the current employee's ID from session
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
                return RedirectToAction("Login", "Account");

            // Get department and committee IDs for the employee
            var employeeDeptId = _repo.GetDepartmentIdByEmployeeId(employeeId.Value);
            var employeeCommitteeIds = _repository.GetCommitteeIdsByEmployeeId(employeeId.Value);

            // Get all announcements
            var allAnnouncements = _announcementrepo.GetAll();

            // Filter announcements based on visibility
            var filtered = allAnnouncements.Where(a =>
                a.VisibleTo == "All"
                || (a.VisibleTo == "Department" && a.VisibleToDepartmentId == employeeDeptId)
                || (a.VisibleTo == "Committee" && employeeCommitteeIds.Contains(a.VisibleToCommitteeId ?? 0))
            ).ToList();

            return View("EmployeeAnnouncement", filtered);
        }



    }
}
