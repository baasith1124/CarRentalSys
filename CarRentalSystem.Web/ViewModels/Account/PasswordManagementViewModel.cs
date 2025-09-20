using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.Account
{
    public class PasswordManagementViewModel
    {
        public bool IsGoogleAccount { get; set; }
        public bool HasPassword { get; set; }
        
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;
        
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
