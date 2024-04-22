using BlogWebApp.Models;
using BlogWebApp.Models.IdentityModel;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using X.PagedList;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public BlogController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("/")]
        public IActionResult Index(int? page, string sortOrder)
        {
            // Get the current user
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                // Handle case when user is not found
                return NotFound();
            }

            // Define default sorting order
            ViewData["CurrentSort"] = sortOrder;
            ViewData["RandomSortParam"] = String.IsNullOrEmpty(sortOrder) ? "random_desc" : "";
            ViewData["PopularitySortParam"] = sortOrder == "Popularity" ? "popularity_desc" : "Popularity";
            ViewData["RecencySortParam"] = sortOrder == "Recency" ? "recency_desc" : "Recency";

            // Determine sorting logic based on sortOrder
            var userBlogsQuery = from blog in _dbContext.Blogs
                                 join category in _dbContext.BlogCategories on blog.BlogCategoryId equals category.Id
                                 where blog.AuthorId == Guid.Parse(user.Id) // Filter blogs by AuthorId
                                 select new BlogVM
                                 {
                                     Id = blog.Id,
                                     Title = blog.Title,
                                     Body = blog.Body,
                                     BlogCategoryId = blog.BlogCategoryId,
                                     PublishedDate = blog.CreationAt,
                                     CategoryName = category.Name, // Populate the category name
                                     BlogImages = blog.BlogImages.Select(bi => new BlogImageVM
                                     {
                                         ImageName = bi.ImageName,
                                         Url = bi.Url
                                     }).Take(1).
                                     ToList(),
                                     TotalComments = _dbContext.Comments.Count(c => c.BlogId == blog.Id),
                                     TotalUpvote = _dbContext.Reactions.Count(r => r.EntityId == blog.Id && r.Type == "Upvote"),
                                     TotalDownvote = _dbContext.Reactions.Count(r => r.EntityId == blog.Id && r.Type == "Downvote"),
                                   

        };

            switch (sortOrder)
            {
                case "random_desc":
                    userBlogsQuery = userBlogsQuery.OrderBy(x => Guid.NewGuid()); // Order randomly
                    break;
                case "Popularity":
                    // Implement popularity sorting logic
                    break;
                case "Recency":
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.PublishedDate); // Order by recency
                    break;
                default:
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.PublishedDate); // Default: order by recency
                    break;
            }

            int pageSize = 10;
            var userBlogs = userBlogsQuery.ToPagedList(page ?? 1, pageSize); // Convert to paged list

            return View(userBlogs); // Pass userBlogs to the view
            //return View();
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
        public IActionResult ManageBlog(int? page)
        {
            // Get the current user
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                // Handle case when user is not found
                return NotFound();
            }
            int pageSize = 2;

            // Retrieve all blogs of the current user including their blog images
            // Define the query to retrieve blogs of the current user including their category details
            var userBlogsQuery = from blog in _dbContext.Blogs
                                 join category in _dbContext.BlogCategories on blog.BlogCategoryId equals category.Id
                                 where blog.AuthorId == Guid.Parse(user.Id) // Filter blogs by AuthorId
                                 select new BlogVM
                                 {
                                     Id = blog.Id,
                                     Title = blog.Title,
                                     Body = blog.Body,
                                     BlogCategoryId = blog.BlogCategoryId,
                                     PublishedDate= blog.CreationAt,
                                     CategoryName = category.Name, // Populate the category name
                                     BlogImages = blog.BlogImages.Select(bi => new BlogImageVM
                                     {
                                         ImageName = bi.ImageName,
                                         Url = bi.Url
                                     }).ToList()
                                 };

            var userBlogs = userBlogsQuery
                .ToPagedList(page ?? 1, pageSize); // Convert to paged list

            return View(userBlogs); // Pass userBlogs to the view
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var blog = await _dbContext.Blogs
                .Include(b => b.BlogImages) // Include related blog images
                .Include(b => b.BlogCategories) // Include related blog category
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            // Populate a view model with the blog data and related entities
            var model = new BlogVM
            {
                Id = blog.Id,
                Title = blog.Title,
                Body = blog.Body,
                BlogCategoryId = blog.BlogCategoryId,
                // Add other properties as needed
                BlogImages = blog.BlogImages.Select(bi => new BlogImageVM
                {
                    ImageName = bi.ImageName,
                    Url = bi.Url
                }).ToList(),
                //CategoryName = blog.BlogCategories. // Retrieve category name
            };
            var categories = _dbContext.BlogCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View(model);
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
                    BlogCategoryId = model.BlogCategoryId,
                    AuthorId = (user == null ? Guid.Empty : (Guid.TryParse(userid, out var userId) ? userId : Guid.Empty)),
                    //AuthorId = model.AuthorId,
                    CreationAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
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
                        ImageName = imageFile.ImageName
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

        [HttpGet("manageblog")]
        public IActionResult GetAllBlog(int? page, string sortOrder)
        {
            // Get the current user
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                // Handle case when user is not found
                return NotFound();
            }

            // Define default sorting order
            ViewData["CurrentSort"] = sortOrder;
            ViewData["RandomSortParam"] = String.IsNullOrEmpty(sortOrder) ? "random_desc" : "";
            ViewData["PopularitySortParam"] = sortOrder == "Popularity" ? "popularity_desc" : "Popularity";
            ViewData["RecencySortParam"] = sortOrder == "Recency" ? "recency_desc" : "Recency";

            // Determine sorting logic based on sortOrder
            var userBlogsQuery = from blog in _dbContext.Blogs
                                 join category in _dbContext.BlogCategories on blog.BlogCategoryId equals category.Id
                                 where blog.AuthorId == Guid.Parse(user.Id) // Filter blogs by AuthorId
                                 select new BlogVM
                                 {
                                     Id = blog.Id,
                                     Title = blog.Title,
                                     Body = blog.Body,
                                     BlogCategoryId = blog.BlogCategoryId,
                                     PublishedDate = blog.CreationAt,
                                     CategoryName = category.Name, // Populate the category name
                                     BlogImages = blog.BlogImages.Select(bi => new BlogImageVM
                                     {
                                         ImageName = bi.ImageName,
                                         Url = bi.Url
                                     }).ToList(),
                                     TotalComments = _dbContext.Comments.Count(c => c.BlogId == blog.Id),
                                     TotalUpvote = _dbContext.Reactions.Count(r=>r.EntityId == blog.Id && r.Type== "Upvote"),
                                     TotalDownvote = _dbContext.Reactions.Count(r=>r.EntityId == blog.Id && r.Type== "Downvote"),
                                 };

            switch (sortOrder)
            {
                case "random_desc":
                    userBlogsQuery = userBlogsQuery.OrderBy(x => Guid.NewGuid()); // Order randomly
                    break;
                case "Popularity":
                    // Implement popularity sorting logic
                    break;
                case "Recency":
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.PublishedDate); // Order by recency
                    break;
                default:
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.PublishedDate); // Default: order by recency
                    break;
            }

            int pageSize = 2;
            var userBlogs = userBlogsQuery.ToPagedList(page ?? 1, pageSize); // Convert to paged list

            return View(userBlogs); // Pass userBlogs to the view
        }
    }
}
