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
            //var categories = new List<BlogCategory>
            //    {
            //        new BlogCategory { Id = 1, Name = "Category 1" },
            //        new BlogCategory { Id = 2, Name = "Category 2" },
            //        new BlogCategory { Id = 3, Name = "Category 3" }
            //    };
            var categories = _dbContext.BlogCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
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

        [HttpPost]
        public async Task<IActionResult> AddBlog(BlogVM model)
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

        [HttpPost("RemoveBlog/{id}")]
        public async Task<IActionResult> RemoveBlog(Guid id)
        {
            // Find the blog by its ID
            var blog = await _dbContext.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound(); // Blog not found
            }

            // Remove associated entities
            // Remove blog images
            var blogImages = _dbContext.BlogImages.Where(bi => bi.BlogId == id);
            _dbContext.BlogImages.RemoveRange(blogImages);

            // Remove comments
            var comments = _dbContext.Comments.Where(c => c.BlogId == id);
            _dbContext.Comments.RemoveRange(comments);

            // Remove notifications (assuming you have a Notifications table related to blogs)
            var notifications = _dbContext.Notifications.Where(n => n.BlogId == id);
            _dbContext.Notifications.RemoveRange(notifications);

            // Remove the blog itself
            _dbContext.Blogs.Remove(blog);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); // Redirect to home page or any other page after removal
        }

    }
}
