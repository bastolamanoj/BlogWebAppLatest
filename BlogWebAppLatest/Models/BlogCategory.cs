using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class BlogCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Navigation properity for blogs
        public ICollection<Blog> Blogs { get; set; }
    }
}
