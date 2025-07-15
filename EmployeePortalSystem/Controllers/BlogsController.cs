using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;
using EmployeePortalSystem.Models;
using System.Reflection.Metadata;

namespace EmployeePortalSystem.Controllers
{
    public class BlogsController : Controller
    {
        private readonly BlogsRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public BlogsController(BlogsRepository repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }
        public IActionResult BlogDetails(int employeeId)
        {
           
            var blogList = _repository.GetBlogDetails(employeeId);
            return View("BlogDetails",blogList);
        }

        //Employee side
        public IActionResult EmployeeBlogDetails(int employeeId)
        {
            var blogs = _repository.GetBlogDetails(employeeId);
            return View("EmployeeBlogDetails", blogs);
        }
        [HttpGet]
        public IActionResult BlogsCreate()
        {
            return View();
        }


        [HttpPost]
        public IActionResult BlogsCreate(BlogViewModel model)
        {
            if (ModelState.IsValid)
            {
              
                string? ImagePath = model.Image;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string? fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                         Path.GetExtension(model.ImageFile.FileName);

                    string imageFullPath = Path.Combine(_environment.WebRootPath, "uploads",fileName);

                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                    }

                    ImagePath = fileName;
                    Console.WriteLine("Uploaded file name: " + model.ImageFile.FileName);
                }
                int? UserId = HttpContext.Session.GetInt32("EmployeeId");

                var blog = new BlogViewModel
                {

                    Title = model.Title,
                    Content = model.Content,
                    Tags = model.Tags,
                    Image = ImagePath, // can be null if no image uploaded
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    AuthorId = UserId

                };
               

                _repository.CreateBlog(blog);

                
                return RedirectToAction("EmployeeBlogDetails", "Blogs");
            }

            TempData["Message1"] = "Blog created successfully!";
            



            return View("BlogsCreate", model);

        }

        

        [HttpGet]
        public IActionResult BlogDelete(int id, string? returnTo)
        {
            var blog = _repository.GetBlogById(id);
            if (blog == null)
                return NotFound();
            ViewBag.ReturnTo = returnTo;
            return View("BlogDelete", blog);
        }

        [HttpPost]
        public IActionResult BlogDeleteConfirmed(int id, string? returnTo)
        {
            var blog=_repository.GetBlogById(id);

            if (blog == null)
                return NotFound();

            int loggedInEmpId = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            string currentDashboard = HttpContext.Session.GetString("CurrentDashboard");

            // Allow if Admin OR Author of the blog
            if (currentDashboard == "Admin" || blog.AuthorId == loggedInEmpId)
            {
                _repository.DeleteBlog(id);
                TempData["Mess"] = "Blog deleted successfully!";


                if (!string.IsNullOrEmpty(returnTo) && returnTo == "Profile")
                {
                    return RedirectToAction("Profile", "MyProfile", new { activeTab = "blogs" });
                }

                if (currentDashboard == "Admin")
                    return RedirectToAction("BlogDetails", "Blogs");
                else
                    return RedirectToAction("EmployeeBlogDetails", "Blogs");
            }
            

            return RedirectToAction("EmployeeBlogDetails", "Blogs");
        }

        //[HttpPost]
        //public IActionResult ToggleLike(int blogId, string? returnTo)
        //{
        //    int empId = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
        //    _repository.ToggleLike(blogId, empId);

        //    if (!string.IsNullOrEmpty(returnTo) && returnTo.ToLower() == "profile")
        //    {
        //        TempData["ActiveTab"] = "blogs";
        //        return RedirectToAction("Profile", "MyProfile");

        //    }

        //    return RedirectToAction("EmployeeBlogDetails");
        //}

        [HttpPost]

        public JsonResult ToggleLike(int blogId,string? returnTo)
        {
            int empId=Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            _repository.ToggleLike(blogId, empId);

            if (!string.IsNullOrEmpty(returnTo) && returnTo.ToLower() == "profile")
            {
                TempData["ActiveTab"] = "blogs";               
            }

            int updatedCount = _repository.GetLikeCount(blogId);
            return Json(updatedCount);
        }


        //comment


        [HttpPost]
        public JsonResult PostComment(int blogId, string commentText, string? returnTo)
        {
            int employeeId = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));

            if (!string.IsNullOrWhiteSpace(commentText))
            {
                _repository.AddComment(blogId, employeeId, commentText);
            }

            if (!string.IsNullOrEmpty(returnTo) && returnTo == "Profile")
            {
                TempData["ActiveTab"] = "blogs";
                
            }
            
            return Json(new
            {
                success = true,
                employeeName = _repository.GetEmployeeName(employeeId),
                commentText,
                createdAt = DateTime.Now.ToString("dd MMM yyyy hh:mm tt")
            });


        }

        [HttpPost]
        public JsonResult DeleteComment(int commentId,string? returnTo)
        {
            int empId = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            string? dashboard = HttpContext.Session.GetString("CurrentDashboard");

            var comment = _repository.GetCommentById(commentId);

            if (comment == null)
            {
                return Json(new { success = false });
            }

            if (comment.EmployeeId != empId && dashboard!="Admin")
            {
                return Json(new { success = false });
            }

            _repository.DeleteComment(commentId);

           

            return Json(new { success = true });
        }

    }
}
