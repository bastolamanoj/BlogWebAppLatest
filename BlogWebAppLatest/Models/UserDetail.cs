using System.ComponentModel.DataAnnotations;
using BlogWebApp.Models.IdentityModel;

namespace BlogWebApp.Models
{
    public class UserDetail
    {
        [Key]
        public Guid Id { get; set; }
        public string? Position { get; set; }

        [MaxLength(500, ErrorMessage = "Your Bio must be at most 500 characters")]
        public string? Bio { get;set; } 
        public string? Address { get; set; }  
        public string? Country { get; set; }  
        public string? ProfileUrl { get; set; }
        public string UserId { get; set; } // Foreign key to associate with a user
      
        [Url(ErrorMessage = "Invalid website URL")]
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } // Navigation property to access the user
    }
}
