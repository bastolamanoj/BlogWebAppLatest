using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class BlogDetailsVM
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        public string? CategoryName { get; set; }
        public DateTime PublishedDate { get; set; }
        public int? BlogCategoryId { get; set; }
        public int? TotalComments { get; set; }
        public int? TotalUpvote { get; set; }
        public int? TotalDownvote { get; set; }
        public string? VoteType { get; set; }   
        public bool? IsVoted { get; set; }
        public string? UserName { get; set; }
        public string? ProfileUrl { get; set; }
        public List<BlogImageVM> BlogImages { get; set; }
        public List<BlogReactions> BlogReactions { get; set; }
        public List<BlogComments> BlogComments { get; set; }
        public UserVM BlogUser { get; set; }

    }
    public class UserVM
    {
        public string? UserName { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Bio { get; set; }
        public string? Position { get; set; }
        public string? Address { get; set; }
    }

    //public class BlogImages
    //{
    //    public string? ImageName { get; set; }
    //    public string Url { get; set; }
    //}

    public class BlogReactions {
        public string Type { get; set; } // "Upvote" or "Downvote"
        public Guid EntityId { get; set; } // Id of the blog or comment
        public string EntityType { get; set; } // "Blog" or "Comment"
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }

    }
    public class BlogComments {
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public Guid BlogId { get; set; }
        public Guid UseId { get; set; }
        public Guid CommentedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public int? TotalUpvote { get; set; }
        public int? TotalDownvote { get; set; }
        public string? VoteType { get; set; }
        public bool? IsVoted { get; set; }
        public string UserName { get; set; }
        public string Url { get; set; }
        public List<CommentReplyVm> CommentReplies { get; set; }
    }

    public class CommentReplyVm
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Guid CommentId { get; set; }
        public int? ParentReplyId { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; }
        public string Url { get; set; }
        public Guid AuthorId { get; set; }
    }
}
