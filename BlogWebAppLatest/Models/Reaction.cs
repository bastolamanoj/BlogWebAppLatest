namespace BlogWebApp.Models
{
    public class Reaction
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Upvote" or "Downvote"
        public Guid EntityId { get; set; } // Id of the blog or comment
        public string EntityType { get; set; } // "Blog" or "Comment"
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }

        public User User { get; set; }
    }
}
