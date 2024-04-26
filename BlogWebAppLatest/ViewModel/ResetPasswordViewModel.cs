using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class ResetPasswordViewModel
    {
        public string? UserId { get; set; }
        //[Required]
        //[EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
