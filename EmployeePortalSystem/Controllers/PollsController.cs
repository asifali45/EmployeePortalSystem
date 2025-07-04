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

        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            TempData["Message"] = "Poll deleted.";
            return RedirectToAction("PollDetails");
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
            var polls = _repo.GetAll();

            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in polls)
            {
                results[poll.PollId] = _repo.GetResults(poll.PollId);
            }

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

            if (_repo.HasVoted(pollId, empId.Value))
            {
                TempData["Error"] = "You have already voted on this poll.";
            }
            else
            {
                var response = new PollResponse
                {
                    PollId = pollId,
                    EmployeeId = empId.Value,
                    SelectedOption = selectedOption,
                    CreatedAt = DateTime.Now
                };

                _repo.SubmitResponse(response);
                TempData["Message"] = "Vote submitted!";
            }

            // Refresh polls and results
            var polls = _repo.GetAll();
            var results = new Dictionary<int, Dictionary<string, int>>();
            foreach (var poll in polls)
            {
                results[poll.PollId] = _repo.GetResults(poll.PollId);
            }

            ViewBag.ResultsPollId = pollId;
            ViewBag.Results = results;

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
