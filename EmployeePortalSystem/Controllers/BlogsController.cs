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
    }
}
