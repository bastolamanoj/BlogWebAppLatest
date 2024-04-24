namespace BlogWebApp.ViewModel
{
    public class ReactionVM
    {
        public string Type { get; set; } // "Upvote" or "Downvote"
        public string EntityId { get; set; } // Id of the blog or comment
        public string EntityType { get; set; }
    }
}
