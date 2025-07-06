using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeePortalSystem.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly AnnouncementRepository _repo;

        public AnnouncementController(AnnouncementRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var announcements = _repo.GetAll();
                                  
            return View(announcements);
     
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Announcement announcement)
        {
            // Remove AnnouncementId if posted accidentally
            ModelState.Remove("AnnouncementId");

            if (!ModelState.IsValid)
                return View(announcement);

            // Set server-side fields
            announcement.PostDate = DateTime.Now;
            announcement.CreatedBy = 1;

            _repo.Add(announcement);

            TempData["Message"] = "Announcement created successfully!";
            return RedirectToAction("Index");
        }



        public IActionResult Edit(int id)
        {
            var announcement = _repo.GetById(id);
            if (announcement == null) return NotFound();

            ViewBag.VisibleToOptions = GetVisibleToOptions();
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
            var existing = _repo.GetById(model.AnnouncementId);
            if (existing == null) return NotFound();

            model.PostDate = existing.PostDate; // Preserve original PostDate
            model.UpdatedBy = 1;
            model.UpdatedAt = DateTime.Now;

            _repo.Update(model);

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var announcement = _repo.GetById(id);
            if (announcement == null) return NotFound();

            return View(announcement);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Delete(id);
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
            var announcements = _repo.GetAll(); // Or filter based on employee, department, etc.
            return View("EmployeeAnnouncement", announcements);
        }


    }
}
