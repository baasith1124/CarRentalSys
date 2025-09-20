using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.Account
{
    public class ProfilePictureViewModel
    {
        public string? CurrentImagePath { get; set; }
        
        [Display(Name = "Profile Picture")]
        [Required(ErrorMessage = "Please select a profile picture")]
        public IFormFile? ProfileImage { get; set; }
    }
}
