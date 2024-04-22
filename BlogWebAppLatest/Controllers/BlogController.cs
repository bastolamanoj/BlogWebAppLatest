using BlogWebApp.Models;
using BlogWebApp.Models.IdentityModel;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public BlogController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext  = dbContext;
            _userManager=   userManager;
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
        public async Task<IActionResult> AddBlog([FromForm] BlogVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;
            if (ModelState.IsValid)
            {
                var blog = new Blog
                {
                    Title = model.Title,
                    Body = model.Body,
                    BlogCategoryId= model.BlogCategoryId,
                    AuthorId = (user == null ? Guid.Empty : (Guid.TryParse(userid, out var userId) ? userId : Guid.Empty)),
                    //AuthorId = model.AuthorId,
                    CreationAt = DateTime.Now,
                    UpdatedAt= DateTime.Now
                };
                await _dbContext.Blogs.AddAsync(blog);
                //await _dbContext.SaveChangesAsync();
                // Create the BlogImage objects
                foreach (var imageFile in model.BlogImages)
                {
                    var blogImage = new BlogImage
                    {
                        BlogId = blog.Id,
                        Url = imageFile.Url, // Assuming you have ImageUrl property in BlogImage model
                        ImageName =imageFile.ImageName
                    };
                    _dbContext.BlogImages.Add(blogImage);
                }
                await _dbContext.SaveChangesAsync();

            }
            return View(model);
        }
    }
}
