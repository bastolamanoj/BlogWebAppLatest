using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using BlogWebApp.Models.IdentityModel;

namespace BlogWebApp.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
        public Guid CommentedBy { get; set; }
        public DateTime CreationDate { get; set; }

        // Navigation property to access the blog associated with the comment
        public Blog Blog { get; set; }
        //public string AuthorId { get; set; }

        // Navigation property to access the user who authored the comment
        public User Author { get; set; }

        // Navigation property to access the notifications associated with the comment
        //public ICollection<Notification> Notifications { get; set; }

        // Navigation property to access the Reaction associated with the comment
        public ICollection<Reaction> Reactions { get; set; }
    }
}
