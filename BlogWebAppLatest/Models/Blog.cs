using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class Blog
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreationAt{ get; set; }
        public DateTime? UpdatedAt{ get; set; }

        public User Author { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Reaction> Reactions { get; set; }
    }
}
