using Microsoft.AspNetCore.Mvc;
using EmployeePortalSystem.Repositories;
using EmployeePortalSystem.ViewModels;

namespace EmployeePortalSystem.Controllers
{
    public class BlogsController : Controller
    {
        private readonly BlogsRepository _repository;

        public BlogsController(BlogsRepository repository)
        {
            _repository = repository;
        }
        public IActionResult BlogDetails()
        {
           
            var blogList = _repository.GetBlogDetails();
            return View("BlogDetails",blogList);
        }

        //Employee side
        public IActionResult EmployeeBlogDetails()
        {
            var blogs = _repository.GetBlogDetails();
            return View("EmployeeBlogDetails", blogs);
        }

        [HttpGet]
        public IActionResult BlogDelete(int id)
        {
            var blog = _repository.GetBlogById(id);
            if (blog == null)
                return NotFound();
            return View("BlogDelete", blog);
        }

        [HttpPost]
        public IActionResult BlogDeleteConfirmed(int id)
        {
            _repository.DeleteBlog(id);

            TempData["Mess"] = "Blog deleted successfully!";

            return RedirectToAction("BlogDetails", "Blogs");
        }

        [HttpPost]
        public IActionResult ToggleLike(int blogId)
        {
            int empId = Convert.ToInt32(HttpContext.Session.GetInt32("EmployeeId"));
            _repository.ToggleLike(blogId, empId);

            return RedirectToAction("EmployeeBlogDetails");
        }
    }
}
