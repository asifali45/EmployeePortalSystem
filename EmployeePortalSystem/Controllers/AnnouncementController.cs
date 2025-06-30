using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeePortalSystem.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IAnnouncementRepository _repo;

        public AnnouncementController(IAnnouncementRepository repo)
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
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"❌ {key}: {error.ErrorMessage}");
                    }
                }
                return View(announcement);
            }

            announcement.PostDate = DateTime.Now;
            announcement.CreatedBy = 1;

            // Debug log
            Console.WriteLine("📌 DEBUG - IsEvent: " + announcement.IsEvent);
            Console.WriteLine("📌 DEBUG - EventDate: " + announcement.EventDate);
            Console.WriteLine("📌 DEBUG - EventTime: " + announcement.EventTime);

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

            model.UpdatedBy = 1;
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
    }
}
