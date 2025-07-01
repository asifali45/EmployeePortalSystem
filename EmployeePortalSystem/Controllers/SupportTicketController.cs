using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Controllers
{
    public class SupportTicketController : Controller
    {
        private readonly SupportTicketRepository _repository;

        public SupportTicketController(SupportTicketRepository repository)
        {
            _repository = repository;
        }

        // ✅ Show ticket list (View: Views/support/SupportTicketList.cshtml)
        public async Task<IActionResult> Index()
        {
            var model = new SupportTicketViewModel
            {
                TicketList = (await _repository.GetAllAsync()).ToList()
            };
            return View("~/Views/support/SupportTicketList.cshtml", model);
        }

        // ✅ Show Raise form (View: Views/support/RaiseTicket.cshtml)
        [HttpGet]
        public IActionResult RaiseTicket()
        {
            return View("~/Views/support/RaiseTicket.cshtml", new SupportTicketViewModel());
        }

        // ✅ Handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RaiseTicket(SupportTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/support/RaiseTicket.cshtml", model);
            }

            var ticket = new SupportTicket
            {
                EmployeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0,
                IssueTitle = model.IssueTitle,
                Description = model.Description,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            await _repository.CreateAsync(ticket);

            TempData["Message"] = "Support ticket submitted successfully!";
            return RedirectToAction("Index");
        }
    }
}
