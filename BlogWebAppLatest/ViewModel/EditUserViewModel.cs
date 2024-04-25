using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class EditUserViewModel
    {
        public string? Id { get; set;}
        [Required]
        public string? UserName { get; set;}
        public string? Bio { get; set; }
        public string? Position { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? ProfileUrl { get; set; }
    }
}
