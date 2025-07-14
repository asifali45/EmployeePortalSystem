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
        public async Task<IActionResult> EmployeeAwardlist()
        {
            var model = new AwardViewModel
            {
                AwardList = (await _repository.GetAllAsync()).ToList()
            };

            return View("EmployeeAwardlist", model);
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

            var recipientName = award.RecipientId.HasValue
    ? await _repository.GetEmployeeNameByIdAsync(award.RecipientId.Value)
    : string.Empty;


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
        // GET: Show confirmation page
        public async Task<IActionResult> Delete(int id)
        {
            var award = await _repository.GetAwardByIdAsync(id);
            if (award == null)
                return NotFound();

            return View(award); // View name should be Delete.cshtml
        }

        [HttpGet]
        public async Task<JsonResult> SearchEmployeeNames(string term)
        {
            var names = await _repository.SearchEmployeeNamesAsync(term);
            return Json(names);
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
            int? finalRecipientId = recipientId > 0 ? recipientId : (int?)null;

            var award = new Award
            {
                Type = model.Type,
                EventDate = model.EventDate.Value,

                RecipientId = finalRecipientId,
                RecipientName = model.RecipientName,
                GivenBy = model.GivenBy,
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
                CreatedBy = model.CreatedBy,
                UpdatedBy = model.UpdatedBy,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(award);
            TempData["Message6"] = "Award added successfully.";
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

            int? recipientId = null;
            if (!string.IsNullOrWhiteSpace(model.RecipientName))
            {
                recipientId = await _repository.GetEmployeeIdByNameAsync(model.RecipientName);
                if (recipientId == 0)
                {
                    ModelState.AddModelError("RecipientName", "Invalid employee name.");
                    return View("AwardForm", model);
                }
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

            TempData["Message6"] = "Award updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: Final delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAwardAsync(id);
            TempData["Message6"] = "Award deleted successfully.";
            return RedirectToAction("Index");
        }

    }

}
