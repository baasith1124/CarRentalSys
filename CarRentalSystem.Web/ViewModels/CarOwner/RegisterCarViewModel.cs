using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.CarOwner
{
    public class RegisterCarViewModel
    {
        [Required(ErrorMessage = "Car name is required")]
        [Display(Name = "Car Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Model")]
        public string Model { get; set; } = null!;

        [Display(Name = "Car Image")]
        public IFormFile? CarImage { get; set; }

        [Required(ErrorMessage = "Available from date is required")]
        [Display(Name = "Available From")]
        [DataType(DataType.DateTime)]
        public DateTime AvailableFrom { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Available to date is required")]
        [Display(Name = "Available To")]
        [DataType(DataType.DateTime)]
        public DateTime AvailableTo { get; set; } = DateTime.Now.AddMonths(1);

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Display(Name = "Features")]
        [StringLength(300, ErrorMessage = "Features cannot exceed 300 characters")]
        public string? Features { get; set; }

        [Display(Name = "Year")]
        [Range(1990, 2025, ErrorMessage = "Year must be between 1990 and 2025")]
        public int? Year { get; set; }

        [Display(Name = "Transmission")]
        public string? Transmission { get; set; }

        [Display(Name = "Fuel Type")]
        public string? FuelType { get; set; }

        [Required(ErrorMessage = "Rate per day is required")]
        [Display(Name = "Rate Per Day")]
        [Range(1, 10000, ErrorMessage = "Rate must be between $1 and $10,000")]
        public decimal RatePerDay { get; set; }

        [Display(Name = "Documents")]
        public IFormFile[]? Documents { get; set; }
    }
}
