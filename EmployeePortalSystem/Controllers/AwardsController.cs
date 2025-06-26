using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Controllers
{
    public class AwardController : Controller
    {
        private readonly AwardRepository _repository;

        public AwardController(AwardRepository repository)
        {
            _repository = repository;
        }

        // Show Award List
        public async Task<IActionResult> Index()
        {
            var model = new AwardViewModel();
            model.AwardList = (await _repository.GetAllAsync()).ToList();
            return View("AwardList", model);
        }

        // Show Add Form
        [HttpGet]
        public IActionResult Create()
        {
            var model = new AwardViewModel
            {
                EventDate = DateTime.Today
            };
            return View("AwardForm", model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var award = await _repository.GetByIdAsync(id);
            if (award == null)
            {
                return NotFound();
            }

            var recipientName = await _repository.GetEmployeeNameByIdAsync(award.RecipientId);

            var model = new AwardViewModel
            {
                AwardId = award.AwardId,
                Type = award.Type,
                EventDate = award.EventDate,
                RecipientName = recipientName,
                GivenBy = award.GivenBy,
                Description = award.Description,
                DisplayOrder = award.DisplayOrder,
                CreatedBy = award.CreatedBy,
                UpdatedBy = award.UpdatedBy
            };

            return View("AwardForm", model); // ✅ reuse AwardForm.cshtml
        }


        // Save Award
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AwardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AwardForm", model);
            }

            int recipientId = await _repository.GetEmployeeIdByNameAsync(model.RecipientName);
            if (recipientId == 0)
            {
                ModelState.AddModelError("RecipientName", "Invalid employee name.");
                return View("AwardForm", model);
            }

            var award = new Award
            {
                Type = model.Type,
                EventDate = model.EventDate.Value,

                RecipientId = recipientId,
                GivenBy = model.GivenBy,
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(award);
            TempData["Message"] = "Award successfully added.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AwardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AwardForm", model);
            }

            int recipientId = await _repository.GetEmployeeIdByNameAsync(model.RecipientName);
            if (recipientId == 0)
            {
                ModelState.AddModelError("RecipientName", "Invalid employee name.");
                return View("AwardForm", model);
            }

            var award = await _repository.GetByIdAsync(model.AwardId);
            if (award == null)
            {
                return NotFound();
            }

            // Update fields
            award.Type = model.Type;
            award.EventDate = model.EventDate.Value;
            award.RecipientId = recipientId;
            award.GivenBy = model.GivenBy;
            award.Description = model.Description;
            award.DisplayOrder = model.DisplayOrder;
            award.UpdatedBy = model.UpdatedBy;
            award.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(award);

            TempData["Message"] = "Award successfully updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var award = await _repository.GetByIdAsync(id);
            if (award == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            TempData["Message"] = "Award successfully deleted.";
            return RedirectToAction("Index");
        }

    }

}
