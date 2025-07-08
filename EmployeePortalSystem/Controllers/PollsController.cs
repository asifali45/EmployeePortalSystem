using EmployeePortalSystem.Context;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalSystem.Controllers
{
    public class PollsController : Controller
    {
        private readonly PollRepository _repo;

        public PollsController(AppDbContext context)
        {
            _repo = new PollRepository(context);
        }

        // ======================= ADMIN =======================

        public IActionResult PollDetails()
        {
            var polls = _repo.GetAll();

            // Collect result summary for all polls
            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in polls)
            {
                results[poll.PollId] = _repo.GetResults(poll.PollId);
            }

            ViewBag.Results = results;
            return View(polls); // Views/Polls/PollDetails.cshtml
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Return Create.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Poll model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.Now;
            model.CreatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;

            _repo.Add(model);

            TempData["Message"] = "Poll created successfully!";
            return RedirectToAction("PollDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "UserAccess");

            var poll = _repo.GetById(id);
            if (poll == null || poll.CreatedBy != empId)
                return Forbid(); // prevent unauthorized access

            _repo.Delete(id);
            TempData["Message"] = "Poll deleted.";
            return RedirectToAction("EmployeePollDetails");
        }

        public IActionResult Results(int id)
        {
            var poll = _repo.GetById(id);
            var results = _repo.GetResults(id);
            var responses = _repo.GetResponsesWithEmployee(id);

            ViewBag.Results = results;
            ViewBag.Responses = responses;

            return View("Results", poll); // Admin view
        }

        // ======================= EMPLOYEE =======================

        public IActionResult EmployeePollDetails()
        {
            var empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "UserAccess");

            var polls = _repo.GetAll();

            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in polls)
            {
                results[poll.PollId] = _repo.GetResults(poll.PollId);
            }

            var selectedOptions = _repo.GetSelectedOptionsForEmployee(empId.Value); // get past votes

            ViewBag.SelectedOptions = selectedOptions;
            ViewBag.Results = results;

            return View(polls); // Views/Polls/EmployeePollDetails.cshtml
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VoteFromList(int pollId, string selectedOption)
        {
            var empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return RedirectToAction("Login", "UserAccess");

            if (!_repo.HasVoted(pollId, empId.Value))
            {
                var response = new PollResponse
                {
                    PollId = pollId,
                    EmployeeId = empId.Value,
                    SelectedOption = selectedOption,
                    CreatedAt = DateTime.Now
                };

                _repo.SubmitResponse(response);
            }

            var polls = _repo.GetAll();
            var selectedOptions = _repo.GetSelectedOptionsForEmployee(empId.Value);

            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in polls)
            {
                results[poll.PollId] = _repo.GetResults(poll.PollId);
            }

            ViewBag.Results = results;
            ViewBag.SelectedOptions = selectedOptions;

            return View("EmployeePollDetails", polls);
        }





        public IActionResult EmployeeResults(int id)
        {
            var poll = _repo.GetById(id);
            var results = _repo.GetResults(id);

            ViewBag.Results = results;
            return View("EmployeeResults", poll); // Employee view
        }
    }
}
