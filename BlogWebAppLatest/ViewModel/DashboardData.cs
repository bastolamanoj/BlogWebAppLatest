
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class DashboardData
    {
        public int TotalBlogPosts { get; set; }
        public int TotalUpvotes { get; set; }
        public int TotalDownvotes { get; set; }
        public int TotalComments { get; set; }
        public List<PopularBlogPost> PopularBlogPosts { get; set; }    
        public List<PopularBloggers> PopularBloggers { get; set; }    
    }

    public class PopularBlogPost {
      public string Title { get;set; }
      public string Body { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ImageUrl { get; set; }
    }

    public class PopularBloggers
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
    }

}