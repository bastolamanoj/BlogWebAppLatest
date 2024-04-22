using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
