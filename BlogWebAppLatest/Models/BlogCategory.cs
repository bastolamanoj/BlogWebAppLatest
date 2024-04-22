using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class BlogCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }= DateTime.Now;

        //Navigation properity for blogs
        //public ICollection<Blog> Blogs { get; set; }
    }
}
