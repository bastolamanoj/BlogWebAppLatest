using BlogWebApp.Models;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public BlogController(ApplicationDbContext dbContext)
        {
            _dbContext  = dbContext;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("blog-details")]
        public IActionResult Details()
        {
            return View();
        }

        [HttpGet("addblog")]
        public IActionResult AddBlog()
        {
            var categories = new List<BlogCategory>
                {
                    new BlogCategory { Id = 1, Name = "Category 1" },
                    new BlogCategory { Id = 2, Name = "Category 2" },
                    new BlogCategory { Id = 3, Name = "Category 3" }
                };

            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            return View();
        }

        [HttpGet("manageblog")]
        public IActionResult ManageBlog()
        {
            var categories = new List<BlogCategory>
                {
                    new BlogCategory { Id = 1, Name = "Category 1" },
                    new BlogCategory { Id = 2, Name = "Category 2" },
                    new BlogCategory { Id = 3, Name = "Category 3" }
                };

            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            return View();
        }



        [HttpPost("AddBlog")]
        public async Task<IActionResult> AddBlog(Blog blog)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.Blogs.AddAsync(blog);
            }
            return View(blog);
        }
    }
}
