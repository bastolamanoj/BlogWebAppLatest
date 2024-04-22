using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModel
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Password Must doesn't match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
