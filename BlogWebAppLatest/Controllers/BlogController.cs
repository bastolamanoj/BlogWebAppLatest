using BlogWebApp.Attributes;
using BlogWebApp.Models;
using BlogWebApp.Models.IdentityModel;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
        public IActionResult Index(int? page, string? sortOrder)
        {
            // Get the current user
            //var user = _userManager.GetUserAsync(User).Result;
            //if (user == null)
            //{
            //    // Handle case when user is not found
            //    return NotFound();
            //}
            //throw new NotImplementedException();

            // Define default sorting order
            ViewData["CurrentSort"] = sortOrder;
            ViewData["RandomSortParam"] = String.IsNullOrEmpty(sortOrder) ? "random_desc" : "";
            ViewData["PopularitySortParam"] = sortOrder == "Popularity" ? "popularity_desc" : "Popularity";
            ViewData["RecencySortParam"] = sortOrder == "Recency" ? "recency_desc" : "Recency";

            // Determine sorting logic based on sortOrder
            var userBlogsQuery = from blog in _dbContext.Blogs
                                 join category in _dbContext.BlogCategories on blog.BlogCategoryId equals category.Id
                                 /*where blog.AuthorId == Guid.Parse(user.Id)*/ // Filter blogs by AuthorId
                                 select new BlogVM
                                 {
                                     Id = blog.Id,
                                     Title = blog.Title,
                                     Body = blog.Body,
                                     BlogCategoryId = blog.BlogCategoryId,
                                     PublishedDate = blog.CreationAt,
                                     CategoryName = category.Name,
                                     UserName = _dbContext.Users
                                                    .Where(x => x.Id == blog.AuthorId.ToString())
                                                    .Select(x => x.DisplayName)
                                                    .FirstOrDefault(),
                                     ProfileUrl = _dbContext.Users
                                                    .Where(x => x.Id == blog.AuthorId.ToString())
                                                    .Select(x => x.ProfileUrl)
                                                    .FirstOrDefault(),
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
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.TotalUpvote - x.TotalDownvote);

                    break;
                case "Recency":
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.PublishedDate); // Order by recency
                    break;
                default:
                    userBlogsQuery = userBlogsQuery.OrderByDescending(x => x.PublishedDate); // Default: order by recency
                    break;
            }

            int pageSize = 3;
            var userBlogs = userBlogsQuery.ToPagedList(page ?? 1, pageSize); // Convert to paged list

            return View(userBlogs); // Pass userBlogs to the view
            //return View();
        }

        [HttpGet("blog-details")]
        public IActionResult Details(string? id)
        {
            if (id == null || id==null)
            {
                // Handle invalid or missing blog id
                return RedirectToAction("Error", "Home"); // Redirect to an error page or another action
            }
            var blogId = Guid.Parse(id);
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
       var blogDetails = _dbContext.Blogs
          .Where(blog => blog.Id == blogId)
          .Join(_dbContext.BlogCategories,
                blog => blog.BlogCategoryId,
                category => category.Id,
                (blog, category) => new { Blog = blog, Category = category })
          .GroupJoin(_dbContext.Comments,
                     bc => bc.Blog.Id,
                     comment => comment.BlogId,
                     (bc, comments) => new BlogDetailsVM
                     {
                         Id = bc.Blog.Id,
                         Title = bc.Blog.Title,
                         Body = bc.Blog.Body,
                         BlogCategoryId = bc.Blog.BlogCategoryId,
                         PublishedDate = bc.Blog.CreationAt,
                         CategoryName = bc.Category.Name, // Populate the category name
                         BlogImages = bc.Blog.BlogImages.Select(bi => new BlogImageVM
                         {
                             ImageName = bi.ImageName,
                             Url = bi.Url
                         }).ToList(),
                         BlogComments = comments.Select(comment => new BlogComments
                         {
                             CommentId = comment.Id,
                             Content = comment.Content,
                             CommentedBy = comment.CommentedBy,
                             CreationDate = comment.CreationDate,
                             TotalUpvote = _dbContext.Reactions.Count(r => r.EntityId == comment.Id && r.Type == "Upvote"),
                             TotalDownvote = _dbContext.Reactions.Count(r => r.EntityId == comment.Id && r.Type == "Downvote"),
                             CommentReplies = _dbContext.CommentReplies
                                  .Where(reply => reply.CommentId == comment.Id)
                                  .Select(reply => new CommentReplyVm
                                  {
                                      Id = reply.Id,
                                      Content = reply.Content,
                                      CommentId = reply.CommentId,
                                      Timestamp = reply.Timestamp,
                                      AuthorId = reply.AuthorId,
                                      UserName = _dbContext.Users
                                          .Where(x => x.Id == reply.AuthorId.ToString())
                                          .Select(x => x.DisplayName)
                                          .FirstOrDefault(),
                                      Url = _dbContext.Users
                                          .Where(x => x.Id == reply.AuthorId.ToString())
                                          .Select(x => x.ProfileUrl)
                                          .FirstOrDefault()
                                  }).ToList()
                         }).ToList(),
                         TotalComments = comments.Count(),
                         TotalUpvote = _dbContext.Reactions.Count(r => r.EntityId == bc.Blog.Id && r.Type == "Upvote"),
                         TotalDownvote = _dbContext.Reactions.Count(r => r.EntityId == bc.Blog.Id && r.Type == "Downvote")
                     })
          .FirstOrDefault();

                var bloguser = (from blog in _dbContext.Blogs
                                join blogUser in _dbContext.Users
                                on blog.AuthorId.ToString() equals blogUser.Id
                                where blog.Id.ToString() == id
                                select new UserVM
                                {
                                    UserName = blogUser.DisplayName,
                                    Position = blogUser.Position,
                                    Bio = blogUser.Bio,
                                    ProfileUrl = blogUser.ProfileUrl,
                                    Address = blogUser.Address,
                                }).FirstOrDefault();


                blogDetails.BlogUser = bloguser as UserVM;

                blogDetails.IsVoted = false;
                if (user != null)
                {
                    var reaction = _dbContext.Reactions.FirstOrDefault(x => x.UserId == Guid.Parse(user.Id) && x.EntityId == Guid.Parse(id));
                    if (reaction != null)
                    {
                        blogDetails.IsVoted = true;
                        blogDetails.VoteType = reaction.Type;
                    }
                }


                if (blogDetails == null)
                {
                    // Handle the case where no blog is found with the provided id
                    return RedirectToAction("Error", "Dashboard"); // Redirect to a not found page or another action
                }

                return View(blogDetails);
            }
            else
            {
              var  blogDetails = _dbContext.Blogs
                    .Where(blog => blog.Id == blogId)
                    .Join(_dbContext.BlogCategories,
                          blog => blog.BlogCategoryId,
                          category => category.Id,
                          (blog, category) => new { Blog = blog, Category = category })
                    .GroupJoin(_dbContext.Comments,
                               bc => bc.Blog.Id,
                               comment => comment.BlogId,
                               (bc, comments) => new BlogDetailsVM
                               {
                                   Id = bc.Blog.Id,
                                   Title = bc.Blog.Title,
                                   Body = bc.Blog.Body,
                                   BlogCategoryId = bc.Blog.BlogCategoryId,
                                   PublishedDate = bc.Blog.CreationAt,
                                   CategoryName = bc.Category.Name, // Populate the category name
                                   BlogImages = bc.Blog.BlogImages.Select(bi => new BlogImageVM
                                   {
                                       ImageName = bi.ImageName,
                                       Url = bi.Url
                                   }).ToList(),
                                   BlogComments = comments.Select(comment => new BlogComments
                                   {
                                       CommentId = comment.Id,
                                       Content = comment.Content,
                                       CommentedBy = comment.CommentedBy,
                                       CreationDate = comment.CreationDate,
                                       TotalUpvote = _dbContext.Reactions.Count(r => r.EntityId == comment.Id && r.Type == "Upvote"),
                                       TotalDownvote = _dbContext.Reactions.Count(r => r.EntityId == comment.Id && r.Type == "Downvote"),
                                       IsVoted = (_dbContext.Reactions.FirstOrDefault(x => x.UserId == Guid.Parse(user.Id) && x.EntityId == comment.Id)) == null ? false : true,
                                       VoteType = _dbContext.Reactions
                                                        .Where(x => x.UserId == Guid.Parse(user.Id) && x.EntityId == comment.Id)
                                                        .Select(a => a.Type)
                                                        .FirstOrDefault() ?? string.Empty,

                                     UserName = _dbContext.Users
                                                        .Where(x => x.Id == comment.CommentedBy.ToString())
                                                        .Select(x => x.DisplayName)
                                                        .FirstOrDefault(),
                                       Url = _dbContext.Users
                                                        .Where(x => x.Id == comment.CommentedBy.ToString())
                                                        .Select(x => x.ProfileUrl)
                                                        .FirstOrDefault(),
                                       //UserName = _dbContext.Users.Where(a => Guid.Parse(a.Id) == comment.CommentedBy).Select(u => u.UserName).FirstOrDefault(),
                                       CommentReplies = _dbContext.CommentReplies
                                        .Where(reply => reply.CommentId == comment.Id)
                                        .Select(reply => new CommentReplyVm
                                        {
                                            Id = reply.Id,
                                            Content = reply.Content,
                                            CommentId = reply.CommentId,
                                            Timestamp = reply.Timestamp,
                                            AuthorId = reply.AuthorId,
                                            UserName = _dbContext.Users
                                                        .Where(x => x.Id == reply.AuthorId.ToString())
                                                        .Select(x => x.DisplayName)
                                                        .FirstOrDefault(),
                                            Url = _dbContext.Users
                                                        .Where(x => x.Id == reply.AuthorId.ToString())
                                                        .Select(x => x.ProfileUrl)
                                                        .FirstOrDefault()

                                            //UserName= _dbContext.Users.Where(a=> Guid.Parse(a.Id) == reply.AuthorId).Select(u=> u.UserName).FirstOrDefault()

                                        }).ToList()
                                   }).ToList(),
                                   TotalComments = comments.Count(),
                                   TotalUpvote = _dbContext.Reactions.Count(r => r.EntityId == bc.Blog.Id && r.Type == "Upvote"),
                                   TotalDownvote = _dbContext.Reactions.Count(r => r.EntityId == bc.Blog.Id && r.Type == "Downvote")
                               })
                    .FirstOrDefault();

                    var bloguser = (from blog in _dbContext.Blogs
                                    join blogUser in _dbContext.Users
                                    on blog.AuthorId.ToString() equals blogUser.Id
                                    where blog.Id.ToString() == id
                                    select new UserVM
                                    {
                                        UserName = blogUser.DisplayName,
                                        Position = blogUser.Position,
                                        Bio = blogUser.Bio,
                                        ProfileUrl = blogUser.ProfileUrl,
                                        Address = blogUser.Address,
                                    }).FirstOrDefault();


                    blogDetails.BlogUser = bloguser as UserVM;

                    blogDetails.IsVoted = false;
                    if (user != null)
                    {
                        var reaction = _dbContext.Reactions.FirstOrDefault(x => x.UserId == Guid.Parse(user.Id) && x.EntityId == Guid.Parse(id));
                        if (reaction != null)
                        {
                            blogDetails.IsVoted = true;
                            blogDetails.VoteType = reaction.Type;
                        }
                    }


                    if (blogDetails == null)
                    {
                        // Handle the case where no blog is found with the provided id
                        return RedirectToAction("Error", "Dashboard"); // Redirect to a not found page or another action
                    }

                    return View(blogDetails);
            }
            return RedirectToAction("Error", "Dashboard");
        }

        [UserAuthorize]
        [HttpGet("addblog")]
        public IActionResult AddBlog()
        {
            var categories = _dbContext.BlogCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [UserAuthorize]
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
            int pageSize = 3;

            // Retrieve all blogs of the current user including their blog images
            // Define the query to retrieve blogs of the current user including their category details
            var userBlogsQuery = from blog in _dbContext.Blogs
                                 join category in _dbContext.BlogCategories on blog.BlogCategoryId equals category.Id
                                 where blog.AuthorId == Guid.Parse(user.Id)
                                   // Filter blogs by AuthorId
                                 select new BlogVM
                                 {
                                     SN=1,
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
                                     }).ToList()
                                 };

            var userBlogs = userBlogsQuery
                .ToPagedList(page ?? 1, pageSize); // Convert to paged list

            return View(userBlogs); // Pass userBlogs to the view
        }

        [UserAuthorize]
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

        [UserAuthorize]
        [HttpPost]
        public async Task<IActionResult> AddBlog(BlogVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;
            var UserId = Guid.Parse(userid);
            if (ModelState.IsValid)
            {
                var blog = new Blog
                {
                    Title = model.Title,
                    Body = model.Body,
                    BlogCategoryId = model.BlogCategoryId,
                    AuthorId = UserId,
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
                try
                {
                await _dbContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RedirectToAction("manageblog", new { page = 1 });

            }
            return View(model);
        }

        [UserAuthorize]
        [HttpPost]
        public async Task<IActionResult> UpdateBlog(BlogVM model)
        {
            var user = await _userManager.GetUserAsync(User);
            var userid = user.Id;
            if (ModelState.IsValid)
            {
                var blog = await _dbContext.Blogs.FindAsync(model.Id);
                if (blog == null)
                {
                    return NotFound();
                }

                // Update blog properties
                blog.Title = model.Title;
                blog.Body = model.Body;
                blog.BlogCategoryId = model.BlogCategoryId;
                blog.AuthorId = Guid.Parse(userid);
                blog.UpdatedAt = DateTime.Now;

                // Delete existing images
                var existingImages = _dbContext.BlogImages.Where(bi => bi.BlogId == blog.Id);
                _dbContext.BlogImages.RemoveRange(existingImages);

                // Add new images
                foreach (var imageFile in model.BlogImages)
                {
                    var blogImage = new BlogImage
                    {
                        BlogId = blog.Id,
                        Url = imageFile.Url, 
                        ImageName = imageFile.ImageName
                    };
                    _dbContext.BlogImages.Add(blogImage);
                }

                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Blog Updated Successfully...";
                return RedirectToAction("manageblog", new { page = 1 });
            }
            return View(model);
        }



        ////[HttpPost("RemoveBlog/{id}")]
        //[HttpPost, ActionName("RemoveBlog")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveBlog(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
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

            return RedirectToAction("ManageBlog", "Blog"); // Redirect to home page or any other page after removal
        }

        [HttpGet]
        public IActionResult GetPopularAndRecentPosts()
        {
            var recentPosts = _dbContext.Blogs
            .OrderByDescending(blog => blog.CreationAt)
            .Take(10)
            .Select(blog => new
            {
                Id = blog.Id,
                Title = blog.Title,
                PublishedDate = blog.CreationAt,
                BlogImage = blog.BlogImages.FirstOrDefault().Url
            })
            .ToList();

            var popularPosts = _dbContext.Blogs
            .OrderByDescending(blog => blog.CreationAt)
            .Take(10)
            .Select(blog => new
            {
                Id = blog.Id,
                Title = blog.Title,
                PublishedDate = blog.CreationAt,
                BlogImage = blog.BlogImages.FirstOrDefault().Url
            })
            .ToList();

            var result = new
            {
                PopularPosts = popularPosts,
                RecentPosts = recentPosts
            };

            return Ok(result);
        }

    }
}
