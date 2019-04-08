
using System.ComponentModel.DataAnnotations;
namespace LAMP.ViewModel
{
    /// <summary>
    /// Class LoginViewModel
    /// </summary>
    public class LoginViewModel:ViewModelBase
    {
        public long UserID { get; set; }
        [Required(ErrorMessage = "Specify email.")]
        [Display(Name = "Email")]        
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Specify password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Keep me logged in")]
        public bool RememberMe { get; set; }
        public string EmailForResetPassword { get; set; }
    }
    /// <summary>
    /// Class changePasswordViewModel
    /// </summary>
    public class changePasswordViewModel
    {
        public long UserID { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
