using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.KYC
{
    public class KYCUploadViewModel
    {
        [Required]
        [Display(Name = "Document Type (NIC/Passport)")]
        public string DocumentType { get; set; } = null!;

        [Required]
        [Display(Name = "Upload Document")]
        public IFormFile Document { get; set; } = null!;
    }
}
