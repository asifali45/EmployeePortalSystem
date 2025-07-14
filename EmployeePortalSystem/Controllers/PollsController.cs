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
            return View(); // Make sure Views/Polls/Create.cshtml exists
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Poll model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.Now;
            model.CreatedBy = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            string currentDashboard = HttpContext.Session.GetString("CurrentDashboard");

            _repo.Add(model);

            TempData["Message"] = "Poll created successfully!";

            
            if (currentDashboard == "Admin")
                return RedirectToAction("PollDetails", "Polls");
            else
                return RedirectToAction("EmployeePollDetails", "Polls");
        }




        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var poll = _repo.GetById(id);
        //    if (poll == null)
        //        return NotFound();

        //    var empId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
        //    var isAdmin = IsAdmin(empId);

        //    // Only allow delete if admin or creator
        //    if (!isAdmin && poll.CreatedBy != empId)
        //    {
        //        TempData["Error"] = "You are not authorized to delete this poll.";
        //        var role = HttpContext.Session.GetString("Role")?.ToLower();
        //        return RedirectToAction(role == "admin" ? "PollDetails" : "EmployeePollDetails");
        //    }


        //    HttpContext.Session.SetString("CurrentDashboard", isAdmin ? "Admin" : "Employee");


        //    return View(poll); // Goes to Views/Polls/Delete.cshtml
        //}


        [HttpGet]
        public IActionResult Delete(int id, string? returnTo)
        {
            var poll = _repo.GetById(id);
            if (poll == null)
                return NotFound();

            var empId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var isAdmin = IsAdmin(empId); // <- this uses the "Role" session string

            if (!isAdmin && poll.CreatedBy != empId)
            {
                TempData["Error"] = "You are not authorized to delete this poll.";
                var role = HttpContext.Session.GetString("Role")?.ToLower();
                return RedirectToAction(role == "admin" ? "PollDetails" : "EmployeePollDetails");
            }

            HttpContext.Session.SetString("CurrentDashboard", isAdmin ? "Admin" : "Employee");
            ViewBag.ReturnTo = returnTo;
            return View(poll); // This should render Delete.cshtml
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id, string? returnTo)
        {
            var empId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            string currentDashboard = HttpContext.Session.GetString("CurrentDashboard");
            var poll = _repo.GetById(id);
            if (poll == null)
                return NotFound();

            if (poll.CreatedBy != empId && !IsAdmin(empId))
                return Forbid();

            _repo.Delete(id);
            TempData["Message"] = "Poll deleted.";

            if (!string.IsNullOrEmpty(returnTo))
            {
                if (returnTo.ToLower() == "profile")
                {
                    TempData["ActiveTab"] = "polls";
                    return RedirectToAction("Profile", "MyProfile");
                }

                if (returnTo == "EmployeePollDetails")
                    return RedirectToAction("EmployeePollDetails", "Polls");

                if (returnTo == "PollDetails")
                    return RedirectToAction("PollDetails", "Polls");
            }

            if (currentDashboard == "Admin")
                return RedirectToAction("PollDetails", "Polls");

            return RedirectToAction("EmployeePollDetails", "Polls");
        }
            //var role = HttpContext.Session.GetString("Role");
            //if (!string.IsNullOrEmpty(role) && role.ToLower() == "admin")
            //    return RedirectToAction("PollDetails");

            //return RedirectToAction("EmployeePollDetails");


        // Helper method to check admin (you can adjust logic accordingly)
        private bool IsAdmin(int empId)
        {
            // Example logic: You can replace this with your actual admin check
            var role = HttpContext.Session.GetString("Role");
            return role != null && role.ToLower() == "admin";

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult VoteFromList(int pollId, string selectedOption, string? returnTo)
        //{
        //    var empId = HttpContext.Session.GetInt32("EmployeeId");
        //    if (empId == null)
        //        return RedirectToAction("Login", "UserAccess");



        //    // Ensure the selected option is not null or empty
        //    if (string.IsNullOrEmpty(selectedOption))
        //    {
        //        TempData["Error"] = "Please select an option before submitting.";

        //        if (returnTo == "DashboardEmployee")
        //            return RedirectToAction("DashboardEmployee", "UserAccess");

        //        if (returnTo == "Profile")
        //        {
        //            TempData["ActiveTab"] = "polls";
        //            return RedirectToAction("Profile", "MyProfile");
        //        }

               
        //        return RedirectToAction("EmployeePollDetails");
        //    }


        //    if (!_repo.HasVoted(pollId, empId.Value))
        //    {
        //        var response = new PollResponse
        //        {
        //            PollId = pollId,
        //            EmployeeId = empId.Value,
        //            SelectedOption = selectedOption,
        //            CreatedAt = DateTime.Now
        //        };

        //        _repo.SubmitResponse(response);
        //    }
        //    if (returnTo == "Profile")
        //    { 
        //        TempData["ActiveTab"] = "polls";
        //        return RedirectToAction("Profile", "MyProfile");
        //    }

        //    if (returnTo == "DashboardEmployee")
        //    {
        //        return RedirectToAction("DashboardEmployee", "UserAccess");
        //    }

        //    return RedirectToAction("EmployeePollDetails");
           
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult VoteFromList(int pollId, string selectedOption, string? returnTo)
        {
            var empId = HttpContext.Session.GetInt32("EmployeeId");
            if (empId == null)
                return Json(new { success = false, message = "Not logged in" });



            // Ensure the selected option is not null or empty
            if (string.IsNullOrEmpty(selectedOption))
            {
                return Json(new { success = false, message = "Please select an option." });

            }


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
           
            return Json(new
            {
                success = true,
                message = "Vote submitted!",
                returnTo = returnTo 
            });

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
