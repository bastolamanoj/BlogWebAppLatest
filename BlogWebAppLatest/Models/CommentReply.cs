using BlogWebApp.Models.IdentityModel;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class CommentReply
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public Guid CommentId { get; set; }
        public int? ParentReplyId { get; set; } // Nullable, indicates parent reply ID
        public DateTime Timestamp { get; set; }
        public Guid AuthorId { get; set; }
        public Comment comment { get; set; }
        public User Author { get; set; }
        // Navigation properties
        //public virtual ICollection<CommentReply> ChildReplies { get; set; }
    }
}
