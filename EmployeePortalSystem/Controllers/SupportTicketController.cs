using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeePortalSystem.Controllers
{
    public class SupportTicketController : Controller
    {
        private readonly SupportTicketRepository _repository;

        public SupportTicketController(SupportTicketRepository repository)
        {
            _repository = repository;
        }

        //  List All Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = await _repository.GetAllAsync(); // ✅ Uses the updated method with Include
            return View("~/Views/Support/SupportTicketList.cshtml", tickets);
        }

        // Show Raise Form
        [HttpGet]
        public async Task<IActionResult> RaiseTicket()
        {
            var model = new SupportTicketViewModel();

            if (HttpContext.Session.GetString("IsAdmin") == "True")
            {
                var employeeNames = await _repository.GetAllEmployeeNamesAsync();
                model.EmployeeNameList = employeeNames.ToList();
            }

            return View("~/Views/support/RaiseTicket.cshtml", model);
        }

        //  Submit Raised Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RaiseTicket(SupportTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.EmployeeNameList = (await _repository.GetAllEmployeeNamesAsync()).ToList();
                return View("~/Views/support/RaiseTicket.cshtml", model);
            }

            int employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;

            if (employeeId == 0 && !string.IsNullOrEmpty(model.EmployeeName))
            {
                employeeId = await _repository.GetEmployeeIdByNameAsync(model.EmployeeName);
            }

            var ticket = new SupportTicket
            {
                EmployeeId = employeeId,
                IssueTitle = model.IssueTitle,
                Description = model.Description,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            await _repository.CreateAsync(ticket);

            TempData["Message"] = "Support ticket submitted successfully!";
            return RedirectToAction("Index");
        }

        // Show Edit Form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _repository.GetByIdAsync(id);
            if (ticket == null) return NotFound();

            // Get employee to extract department
            var employee = await _repository.GetEmployeeByIdAsync(ticket.EmployeeId);
            var departmentId = employee?.DepartmentId ?? 0;

            var viewModel = new SupportTicketViewModel
            {
                TicketId = ticket.TicketId,
                IssueTitle = ticket.IssueTitle,
                Description = ticket.Description,
                Status = ticket.Status,
                Response = ticket.Response ?? "",
                AssignedTo = ticket.AssignedToName ?? "",
                EscalationName = ticket.EscalatedToName ?? "",
                EscalationLevel = ticket.EscalationLevel.ToString(),
                EmployeeName = employee?.Name ?? "",
                DepartmentId = departmentId,

                DepartmentList = (await _repository.GetDepartmentsAsync())
    .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name })
    .ToList(),


                FilteredEmployees = (await _repository.GetEmployeesByDepartmentAsync(departmentId))
    .Select(e => new SelectListItem { Value = e.EmployeeId.ToString(), Text = e.Name })
    .ToList()


            };

            return View("~/Views/Support/EditTicket.cshtml", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeesByDepartmentId(int id)
        {
            var employees = await _repository.GetEmployeesByDepartmentIdAsync(id);
            return Json(employees.Select(e => new { e.EmployeeId, e.Name }));
        }

        [HttpGet]
        public async Task<JsonResult> GetEmployeesByDepartment(int departmentId)
        {
            var employees = await _repository.GetEmployeesByDepartmentAsync(departmentId);
            var list = employees.Select(e => new { name = e.Name }).ToList();
            return Json(list);
        }



        //  Save Edited Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicket(SupportTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.EmployeeNameList = (await _repository.GetAllEmployeeNamesAsync()).ToList();

                return View("~/Views/support/EditTicket.cshtml", model);
            }

            var ticket = await _repository.GetByIdAsync(model.TicketId);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = model.Status;
            ticket.Response = model.Response;
            ticket.AssignedTo = int.TryParse(model.AssignedTo, out int assigneeId) ? assigneeId : null;
            ticket.EscalatedTo = int.TryParse(model.EscalationName, out int escalateId) ? escalateId : null;

            ticket.EscalationLevel = int.TryParse(model.EscalationLevel, out int level) ? level : 0;
            ticket.UpdatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            ticket.UpdatedAt = DateTime.Now;

            await _repository.UpdateTicketAsync(ticket);
            TempData["Message"] = "Ticket updated successfully!";
            return RedirectToAction("Index");
        }

        //  Update ticket directly (Optional)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicket(SupportTicket model)
        {
            model.UpdatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            model.UpdatedAt = DateTime.Now;

            await _repository.UpdateTicketAsync(model);
            TempData["Message"] = "Ticket updated successfully!";
            return RedirectToAction("Index");
        }

        


    }
}
