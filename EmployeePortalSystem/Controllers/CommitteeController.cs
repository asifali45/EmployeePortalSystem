﻿using EmployeePortalSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Models;
using EmployeePortalSystem.ViewModels;
using System.Xml.Linq;

namespace EmployeePortalSystem.Controllers
{
    public class CommitteeController : Controller
    {
        private readonly CommitteeRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public CommitteeController(CommitteeRepository repository,IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult CommitteeDetails()
        {
            
            var committeeList = _repository.GetAllCommittees();


            return View("CommitteeDetails", committeeList);
        }

        [HttpGet]
        public IActionResult EmployeeCommitteeDetails()
        {

            var committeeList = _repository.GetAllCommittees();



            return View("EmployeeCommitteeDetails", committeeList);
        }

        [HttpGet]
        public IActionResult CreateEditCommittee()
        {
            
            var employee=_repository.GetAllEmployees();
            ViewBag.Employees = employee;
            return View();
        }

      

        [HttpPost]
        public IActionResult CreateEditCommittee(CommitteeViewModel model)
        {
           

            if (ModelState.IsValid)
            {
                string? logoPath = model.Logo;

                if (model.LogoFile != null && model.LogoFile.Length > 0)
                {
                    string? fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                         Path.GetExtension(model.LogoFile.FileName);

                    string imageFullPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        model.LogoFile.CopyTo(stream);
                    }

                    logoPath = fileName;
                    Console.WriteLine("Uploaded file name: " + model.LogoFile.FileName);
                }

                int? UserId = HttpContext.Session.GetInt32("EmployeeId");

                var committee = new Committee
                {
                   
                    Name = model.Name,
                    Type = model.Type,
                    HeadId = model.HeadId,
                    Logo = logoPath, // can be null if no image uploaded
                    Description = model.Description
                   
                };

                if (model.CommitteeId == null || model.CommitteeId == 0)
                {
                    // New record
                    committee.CreatedBy = UserId;
                    committee.CreatedAt = DateTime.Now;
                    committee.UpdatedBy = UserId;
                    committee.UpdatedAt = DateTime.Now;

                    _repository.CreateCommittee(committee);
                    TempData["message2"] = "Committee created successfully.";
                    
                }
                else
                {
                    committee.CommitteeId = model.CommitteeId.Value; 
                    committee.UpdatedBy = UserId;
                    committee.UpdatedAt = DateTime.Now;

                    _repository.UpdateCommittee(committee);
                    TempData["message2"] = "Committee updated successfully.";
                    
                }

                return RedirectToAction("CommitteeDetails");
            }
           
            ViewBag.Employees = _repository.GetAllEmployees();
            return View("CreateEditCommittee", model);
        }

        [HttpGet]
        public IActionResult CommitteeDelete(int cid)
        {
            var committee = _repository.GetCommitteeById(cid);
            if (committee == null)
                return NotFound();
            return View("CommitteeDelete", committee);
        }

        [HttpPost]
        public IActionResult CommitteeDeleteConfirmed(int cid)
        {
            _repository.DeleteCommittee(cid);

            TempData["message2"] = "Committee deleted successfully!";

            return RedirectToAction("CommitteeDetails","Committee");
        }

        [HttpGet]
        public IActionResult EditCommittee(int id)
        {
            var committee = _repository.GetCommitteeById(id);
            if (committee == null)
            {
                return NotFound();
            }

            var headEmployee = _repository.GetEmployeeById(committee.HeadId ?? 0);
            var model = new CommitteeViewModel
            {
                CommitteeId = committee.CommitteeId,
                Name = committee.Name,
                Type = committee.Type,
                HeadId = committee.HeadId,
                Description = committee.Description,

                Logo = committee.Logo
                // Don't assign logoPath here — let them re-upload if needed

            };
            ViewBag.HeadName = headEmployee?.Name;

            ViewBag.Employees = _repository.GetAllEmployees();           
            return View("CreateEditCommittee", model); 
        }


        [HttpGet]
        public IActionResult CommitteeMembers(int id)
        {
            var committee = _repository.GetCommitteeById(id);
            if (committee == null)
                return NotFound("Committee not found");

            var members = _repository.GetCommitteeMembersByCommitteeId(id);
            ViewBag.CommitteeId = id;
            ViewBag.CommitteeName = committee.Name;
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");
            ViewBag.IsCommitteeHead = (committee.HeadId == employeeId);
            return View("CommitteeMemberDetails", members);
        }

        [HttpGet]
        public IActionResult EmployeeCommitteeMembers(int id)
        {
            var committee = _repository.GetCommitteeById(id);
            if (committee == null)
                return NotFound("Committee not found");

            var members = _repository.GetCommitteeMembersByCommitteeId(id);
            ViewBag.CommitteeId = id;
            ViewBag.CommitteeName = committee.Name;
            int? employeeId = HttpContext.Session.GetInt32("EmployeeId");
            ViewBag.IsCommitteeHead = (committee.HeadId == employeeId);
            return View("EmployeeCommitteeMemberDetails", members);
        }


        [HttpGet]
        public IActionResult AddMember(int committeeId)
        {
            var committee = _repository.GetCommitteeById(committeeId); // for name
            var model = new CommitteeMemberInsertionViewModel
            {
                CommitteeId = committeeId,
                CommitteeName = committee.Name,
                Employees = _repository.GetAllEmployees(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddMember(CommitteeMemberInsertionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = _repository.GetAllEmployees();
                return View("AddMember", model);
            }

            string currentDashboard = HttpContext.Session.GetString("CurrentDashboard");
            int? UserId = HttpContext.Session.GetInt32("EmployeeId");

            var existingMember = _repository.GetCommitteeMember(model.CommitteeId, model.EmployeeId);

            if (existingMember != null)
            {
                // It's an edit
                existingMember.UpdatedAt = DateTime.Now;
                existingMember.UpdatedBy = UserId;
                _repository.UpdateCommitteeMember(existingMember);

                TempData["message3"] = "Member updated successfully.";
            }
            else
            {
                // It's a new insert
                var newMember = new CommitteeMember
                {
                    CommitteeId = model.CommitteeId,
                    EmployeeId = model.EmployeeId,
                    CreatedBy = UserId ,
                    UpdatedBy = UserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _repository.AddCommitteeMember(newMember);
                TempData["message3"] = "Member added successfully.";
            }
            if (currentDashboard == "Employee")
            {
                return RedirectToAction("EmployeeCommitteeMembers", new { id = model.CommitteeId });
            }
            return RedirectToAction("CommitteeMembers", new { id = model.CommitteeId });
        }
        [HttpGet]
        public IActionResult EditMember(int id)
        {
            var member = _repository.GetCommitteeMemberById(id);
            if (member == null)
                return NotFound();

            var committee = _repository.GetCommitteeById(member.CommitteeId);

            var model = new CommitteeMemberInsertionViewModel
            {
                CommitteeId = member.CommitteeId,
                EmployeeId = member.EmployeeId,
                CommitteeName = committee?.Name,
                Employees = _repository.GetAllEmployees()
            };

            return View("AddMember", model);

        }

        [HttpGet]
        public IActionResult DeleteCommitteeMember(int committeeMemberId)
        {
            var member = _repository.GetCommitteeMemberViewModelById(committeeMemberId);
            if (member == null) return NotFound();

            return View("DeleteCommitteeMember", member);
        }

        [HttpPost]
        public IActionResult DeleteCommitteeMemberConfirmed(int committeeMemberId, int committeeId)
        {
            string currentDashboard = HttpContext.Session.GetString("CurrentDashboard");
            _repository.DeleteCommitteeMember(committeeMemberId);
            TempData["message3"] = "Member deleted successfully.";
            if (currentDashboard == "Employee")
            {
                return RedirectToAction("EmployeeCommitteeMembers", new { id = committeeId });
            }
            return RedirectToAction("CommitteeMembers", new { id = committeeId });
        }

        [HttpGet]
        public JsonResult SearchAvailableEmployees(int committeeId, string term)
        {
            var employees = _repository.SearchAvailableEmployees(committeeId, term);
            return Json(employees);
        }

        [HttpGet]
        public JsonResult SearchCurrentEmployeesByName(string term)
        {
            var employees = _repository.GetAllEmployees()
             .Where(e => e.IsCurrentEmployee && e.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
             .Select(e => new { employeeId = e.EmployeeId, name = e.Name })
             .ToList(); 

            return Json(employees);
        }
    }

}
