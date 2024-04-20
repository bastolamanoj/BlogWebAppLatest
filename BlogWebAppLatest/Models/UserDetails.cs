using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class UserDetails
    {
        [Key]
        public int Id { get; set; }
        public string? Position { get; set; }
        public string? Bio { get;set; } 
        public string? Email { get; set; }  
        public string? Address { get; set; }  
        public string? ProfileUrl { get; set; }
        public string UserId { get; set; } // Foreign key to associate with a user
      
        [Url(ErrorMessage = "Invalid website URL")]
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public User User { get; set; } // Navigation property to access the user
    }
}
