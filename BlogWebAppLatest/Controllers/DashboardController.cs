using BlogWebApp.Attributes;
using BlogWebApp.Models;
using BlogWebApp.Models.IdentityModel;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Diagnostics;

namespace BlogWebApp.Controllers
{
    [UserAuthorize]
    public class DashboardController : Controller
    { 
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager; 

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext dbcontext,
            UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbcontext;
            _userManager=   userManager;
        }

        [HttpGet("dashboard")]
        public IActionResult Index(int? month = null)
        {
            var dashboardData = new DashboardData();

            // Get all-time data
            dashboardData.TotalBlogPosts = _dbContext.Blogs.Count();
            dashboardData.TotalUpvotes = _dbContext.Reactions.Count(a => a.Type == "Upvote");
            dashboardData.TotalDownvotes = _dbContext.Reactions.Count(a => a.Type == "Downvote");
            dashboardData.TotalComments = _dbContext.Comments.Count();

            dashboardData.PopularBlogPosts = _dbContext.Blogs
             .OrderByDescending(post => post.Comments.Count)
            .ThenByDescending(post => post.Reactions.Count)
            .Take(10)
            .Select(post => new PopularBlogPost
            {
                Title = post.Title,
                Body = post.Body,
                PublishedDate = post.CreationAt
                //ImageUrl = post.BlogImages.FirstOrDefault()
            })
            .ToList();

            // Filter if month is supplied
            if (month.HasValue && month >= 1 && month <= 12)
            {
                var year = DateTime.Today.Year; // Assuming current year
                var startOfMonth = new DateTime(year, month.Value, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                // Calculate monthly counts
                dashboardData.TotalBlogPosts = _dbContext.Blogs
                    .Count(post => post.CreationAt >= startOfMonth && post.CreationAt <= endOfMonth);
                dashboardData.TotalUpvotes = _dbContext.Reactions
                    .Count(a => a.Type == "Upvote" && a.CreationDate >= startOfMonth && a.CreationDate <= endOfMonth);
                dashboardData.TotalDownvotes = _dbContext.Reactions
                    .Count(post => post.Type == "Downvote" && post.CreationDate >= startOfMonth && post.CreationDate <= endOfMonth);
                dashboardData.TotalComments = _dbContext.Comments
                    .Count(comment => comment.CreationDate >= startOfMonth && comment.CreationDate <= endOfMonth);
            }

            return View(dashboardData);
        }


        public IActionResult Profile()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public DashboardData GetDashboardData(DateTime? month = null)
        {
            var dashboardData = new DashboardData();

            // Get all-time data
            dashboardData.TotalBlogPosts = _dbContext.Blogs.Count();
            dashboardData.TotalUpvotes = _dbContext.Reactions.Where(a=> a.Type=="Upvote").Count();
            dashboardData.TotalDownvotes = _dbContext.Reactions.Where(a => a.Type == "Downvote").Count();
            dashboardData.TotalComments = _dbContext.Comments.Count();

            // Filter if month is supplied
            if (month.HasValue)
            {
                var startOfMonth = new DateTime(month.Value.Year, month.Value.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                // Calculate monthly counts
                dashboardData.TotalBlogPosts = _dbContext.Blogs
                    .Count(post => post.CreationAt >= startOfMonth && post.CreationAt <= endOfMonth);
                dashboardData.TotalUpvotes = _dbContext.Reactions
                .Where(a => a.Type == "Downvote" && a.CreationDate >= startOfMonth && a.CreationDate <= endOfMonth).Count();
                dashboardData.TotalDownvotes = _dbContext.Reactions
                    .Where(post => post.Type == "Downvote" && post.CreationDate >= startOfMonth && post.CreationDate <= endOfMonth).Count();
                   
                dashboardData.TotalComments = _dbContext.Comments
                    .Count(comment => comment.CreationDate >= startOfMonth && comment.CreationDate <= endOfMonth);
            }

            return dashboardData;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardDataForBlogger(int? month = null)
        {
            var dashboardData = new DashboardData();
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // If user is not logged in, return an error or handle it appropriately
                return BadRequest("User not found.");
            }

            var userId = user.Id;

            // Get all-time data if userId is not provided
            if (month==null)
            {
                dashboardData.TotalBlogPosts = await _dbContext.Blogs
                 .Where(b => b.AuthorId.ToString() == userId)
                 .CountAsync();

                var userBlogIds = await _dbContext.Blogs
                    .Where(b => b.AuthorId.ToString() == userId)
                    .Select(b => b.Id)
                    .ToListAsync();

                dashboardData.TotalUpvotes = await _dbContext.Reactions
                    .Where(r => userBlogIds.Contains(r.EntityId) && r.Type == "Upvote")
                    .CountAsync();

                dashboardData.TotalDownvotes = await _dbContext.Reactions
                    .Where(r => userBlogIds.Contains(r.EntityId) && r.Type == "Downvote")
                    .CountAsync();

                dashboardData.TotalComments = await _dbContext.Comments
                    .Where(c => userBlogIds.Contains(c.BlogId))
                    .CountAsync();
            }
            else // Filter by userId if provided
            {
                // Filter by userId
                var userBlogsQuery = _dbContext.Blogs.Where(blog => blog.AuthorId.ToString() == userId);

                // Filter by month if provided
                if (month.HasValue && month >= 1 && month <= 12)
                {
                    var startOfMonth = new DateTime(DateTime.Now.Year, month.Value, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                    userBlogsQuery = userBlogsQuery.Where(post => post.CreationAt >= startOfMonth && post.CreationAt <= endOfMonth);
            

                // Calculate counts
                dashboardData.TotalBlogPosts = await userBlogsQuery.CountAsync();
                dashboardData.TotalUpvotes = await _dbContext.Reactions.Where(a => a.Type == "Upvote" && a.UserId.ToString() == userId && a.CreationDate >= startOfMonth && a.CreationDate <= endOfMonth).CountAsync();
                dashboardData.TotalDownvotes = await _dbContext.Reactions.Where(a => a.Type == "Downvote" && a.UserId.ToString() == userId && a.CreationDate >= startOfMonth && a.CreationDate <= endOfMonth).CountAsync();
                dashboardData.TotalComments = await _dbContext.Comments.CountAsync(comment => comment.CommentedBy.ToString() == userId && comment.CreationDate >= startOfMonth && comment.CreationDate <= endOfMonth);
                }
            }

            return Ok(new { dashboardData = dashboardData });
        }

        [HttpGet]
        public async Task<IActionResult> GetTopBloggerUsers(int? month = null)
        {
            if (month.HasValue && month >= 1 && month <= 12) // Validating month input
            {
                var year = DateTime.UtcNow.Year; // Assuming the current year
                var startOfMonth = new DateTime(year, month.Value, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                //var query = (from user in _dbContext.Users
                //             join reaction in _dbContext.Reactions
                //             on user.Id equals reaction.UserId.ToString()
                //             select new
                //             {
                //                 Bio= user.Bio,
                //                 DisplayName= user.DisplayName,
                //                 ProfileUrl = user.ProfileUrl,
                //                 UserId = user.Id,
                //                 UserName = user.DisplayName,
                //                 ReactionCount = _dbContext.Reactions.Count(r => r.UserId.ToString() == user.Id)
                //             })
                //          .OrderByDescending(u => u.ReactionCount)
                //          .Take(10);

                //var topUsers = await query.ToListAsync();

                var topUsers = (from r in _dbContext.Reactions
                                join u in _dbContext.Users on r.UserId.ToString() equals u.Id
                                where r.Type == "Upvote" &&
                                      r.CreationDate >= startOfMonth &&
                                      r.CreationDate <= endOfMonth
                                group r by new { u.ProfileUrl, u.Id, u.Bio, u.DisplayName } into g
                                orderby g.Count() descending
                                select new
                                {
                                    UpvoteCount = g.Count(),
                                    ProfileUrl = g.Key.ProfileUrl,
                                    UserId = g.Key.Id,
                                    Bio = g.Key.Bio,
                                    DisplayName = g.Key.DisplayName
                                }).Take(10).ToList();
                return Ok(topUsers);
            }
            else
            {
                var query = _dbContext.Users
                    .OrderByDescending(u =>
                        _dbContext.Blogs.Count(b => b.AuthorId.ToString() == u.Id) +
                        _dbContext.Comments.Count(c => c.CommentedBy.ToString() == u.Id))
                    .Take(10);

                var topUsers = await query.ToListAsync();

                return Ok(topUsers);
            }
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetTopBlogs(int? month = null)
        {
            var query = _dbContext.Blogs.AsQueryable();

            if (month.HasValue && month >= 1 && month <= 12)
            {
                // Construct start and end dates based on the month provided
                var year = DateTime.Now.Year; // Assuming current year
                var startOfMonth = new DateTime(year, month.Value, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                query = query.Where(b => b.CreationAt >= startOfMonth && b.CreationAt <= endOfMonth);
            }

            var topBlogs = await query.OrderByDescending(b =>
                    (_dbContext.Reactions.Count(r => r.EntityId == b.Id && r.Type == "Upvote") -
                    _dbContext.Reactions.Count(r => r.EntityId == b.Id && r.Type == "Downvote")) +
                    _dbContext.Comments.Count(c => c.BlogId == b.Id))
                 .Select(blog => new
                 {
                     Id = blog.Id,
                     Title = blog.Title,
                     PublishedDate = blog.CreationAt,
                     BlogImage = blog.BlogImages.FirstOrDefault().Url
                 })
                 .Take(10)
                .ToListAsync();

        //    var popularPosts = _dbContext.Blogs
        // .OrderByDescending(blog => blog.CreationAt)
        // .Take(10)
        //;

            return Ok(topBlogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please Fill Required Field";
                return View("profile");
            }

            var user = await _userManager.FindByIdAsync(viewModel.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Please Fill Required Field";
                return RedirectToAction("profile");
                //return NotFound();
            }

            user.DisplayName = viewModel.UserName;
            user.UserName = viewModel.Email;
            user.Email = viewModel.Email;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.Address = viewModel.Address;
            user.Position = viewModel.Position;
            user.Bio = viewModel.Bio;
            user.Country = viewModel.Country;
            user.ProfileUrl = viewModel.ProfileUrl;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User Details updated Successfully.";
                return View("profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                return View("profile");
            }

            return null;
        }

    }
}

