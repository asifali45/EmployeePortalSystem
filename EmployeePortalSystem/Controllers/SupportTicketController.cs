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
            var allTickets = await _repository.GetAllAsync();
            return View("~/Views/Support/SupportTicketList.cshtml", allTickets);
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeTicket()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var empTickets = await _repository.GetTicketsByEmployeeIdAsync(employeeId);
            return View("~/Views/Support/EmployeeTicket.cshtml", empTickets); 
        }
        public IActionResult BackToTickets()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            return RedirectToAction("EmployeeTicket"); // employee history
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

            return View("~/Views/Support/RaiseTicket.cshtml", model);  // ✅ Shared by both
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
                Type = model.Type,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            await _repository.CreateAsync(ticket);

            TempData["Message"] = "Support ticket submitted successfully!";
            return RedirectToAction("EmployeeTicket");
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
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _repository.GetByIdAsync(id);
            if (ticket == null) return NotFound();

            var model = new SupportTicketViewModel
            {
                TicketId = ticket.TicketId,
                IssueTitle = ticket.IssueTitle,
                Description = ticket.Description,
                Type = ticket.Type,
                Status = ticket.Status,
                Response = ticket.Response,
                DepartmentList = (await _repository.GetDepartmentsAsync())
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.Name
                    }).ToList(),
                AllEmployees = (await _repository.GetEmployeesByDepartmentAsync(0))
                    .Select(e => new SupportEmployeeListItem
                    {
                        Name = e.Name,
                        DepartmentId = (int)(e.DepartmentId ?? 0)
                    }).ToList()
            };

            return View("~/Views/Support/EditTicket.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTicket(SupportTicketViewModel model)
        {
            // ✅ Conditional Validation
            if (model.Status == "Assigned")
            {
                if (string.IsNullOrWhiteSpace(model.AssignedTo))
                    ModelState.AddModelError("AssignedTo", "Assigned To is required when status is Assigned.");

                // ✅ Clear unrelated
                ModelState.Remove("EscalationName");
            }
            else if (model.Status == "Escalated")
            {
                if (string.IsNullOrWhiteSpace(model.EscalationName))
                    ModelState.AddModelError("EscalationName", "Escalation Name is required when status is Escalated.");

                // ✅ Clear unrelated
                ModelState.Remove("AssignedTo");
            }
            else
            {
                // ✅ Neither Assigned nor Escalated — remove both
                ModelState.Remove("AssignedTo");
                ModelState.Remove("EscalationName");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var e in error.Value.Errors)
                    {
                        Console.WriteLine($"Error: {e.ErrorMessage}");
                    }
                }
                // repopulate dropdowns
                model.DepartmentList = (await _repository.GetDepartmentsAsync())
                    .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name }).ToList();

                model.AllEmployees = (await _repository.GetEmployeesByDepartmentAsync(0))
                    .Select(e => new SupportEmployeeListItem
                    {
                        Name = e.Name,
                        DepartmentId = (int)(e.DepartmentId ?? 0)

                    }).ToList();

                return View("~/Views/Support/EditTicket.cshtml", model);
            }

            var ticket = await _repository.GetByIdAsync(model.TicketId);
            if (ticket == null) return NotFound();

            ticket.Status = model.Status;
            ticket.Response = model.Response;
            if (!string.IsNullOrEmpty(model.AssignedTo))
            {
                ticket.AssignedTo = await _repository.GetEmployeeIdByNameAsync(model.AssignedTo);
            }

            // EscalatedTo: only update if not empty
            if (!string.IsNullOrEmpty(model.EscalationName))
            {
                ticket.EscalatedTo = await _repository.GetEmployeeIdByNameAsync(model.EscalationName);
            }
            ticket.EscalationLevel = int.TryParse(model.EscalationLevel, out int level) ? level : 0;
            ticket.UpdatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            ticket.UpdatedAt = DateTime.Now;

            await _repository.UpdateTicketAsync(ticket);

            TempData["Message"] = "Ticket updated successfully!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> AssignedTickets()
        {
            int empId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var tickets = await _repository.GetAssignedTicketsAsync(empId);  // this returns List<SupportTicket>

            // Convert to ViewModel list
            var viewModelList = tickets.Select(t => new SupportTicketViewModel
            {
                TicketId = t.TicketId,
                IssueTitle = t.IssueTitle,
                Description = t.Description,
                Status = t.Status,

                Response = t.Response,
                Resolved = t.Resolved

            }).ToList();

            return View("~/Views/Support/AssignedTickets.cshtml", viewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResolution(int TicketId, string Resolved)
        {

            var updatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;

            await _repository.UpdateResolvedAsync(TicketId, Resolved, updatedBy);

//             var ticket = await _repository.GetByIdAsync(TicketId);
//             if (ticket == null)
//                 return NotFound();

//             ticket.Resolved = Resolved;
//             ticket.UpdatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
//             ticket.UpdatedAt = DateTime.Now;

//             await _repository.UpdateTicketAsync(ticket);


            TempData["Message"] = "Resolution updated successfully!";
            return RedirectToAction("AssignedTickets");
        }















    }
}
