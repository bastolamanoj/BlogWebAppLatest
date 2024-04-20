using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace BlogWebApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreationDate { get; set; }

        // Navigation property to access the blog associated with the comment
        public Blog Blog { get; set; }

        // Navigation property to access the user who authored the comment
        public User Author { get; set; }

        // Navigation property to access the notifications associated with the comment
        public ICollection<Notification> Notifications { get; set; }
    }
}
