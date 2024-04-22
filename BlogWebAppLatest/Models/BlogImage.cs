using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class BlogImage
    {
        [Key]
        public int Id { get; set; }
        public string? ImageName { get; set; }
        public string Url { get; set; }
        //public int Position { get; set; } // Position of the image within the blog post
        public Guid BlogId { get; set; } // Foreign key to associate with a blog
        public Blog Blog { get; set; } // Navigation property to access the blog
    }
}
