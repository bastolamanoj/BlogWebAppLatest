using BlogWebApp.Models;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace BlogWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext dbcontext)
        {
            _logger = logger;
            _dbContext = dbcontext;
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

            dashboardData.PopularBloggers = _dbContext.Blogs
             .OrderByDescending(post => post.Comments.Count)
            .ThenByDescending(post => post.Reactions.Count)
            .Take(10)
            .Select(post => new PopularBlogPost
            {
                Title = post.Title,
                Body = post.Body,
                PublishedDate = post.CreationAt,
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
    }
}

