using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class AdminEditCarViewModel
    {
        public Guid CarId { get; set; }

        [Required(ErrorMessage = "Car name is required")]
        [Display(Name = "Car Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Model is required")]
        [Display(Name = "Model")]
        public string Model { get; set; } = null!;

        [Display(Name = "Car Image")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Available from date is required")]
        [Display(Name = "Available From")]
        [DataType(DataType.Date)]
        public DateTime AvailableFrom { get; set; }

        [Required(ErrorMessage = "Available to date is required")]
        [Display(Name = "Available To")]
        [DataType(DataType.Date)]
        public DateTime AvailableTo { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Display(Name = "Features")]
        [StringLength(300, ErrorMessage = "Features cannot exceed 300 characters")]
        public string? Features { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Display(Name = "Year")]
        [Range(1990, 2025, ErrorMessage = "Year must be between 1990 and 2025")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Transmission is required")]
        [Display(Name = "Transmission")]
        public string Transmission { get; set; } = null!;

        [Required(ErrorMessage = "Fuel type is required")]
        [Display(Name = "Fuel Type")]
        public string FuelType { get; set; } = null!;

        [Required(ErrorMessage = "Rate per day is required")]
        [Display(Name = "Rate Per Day")]
        [Range(1, 10000, ErrorMessage = "Rate must be between $1 and $10,000")]
        public decimal RatePerDay { get; set; }

        [Required(ErrorMessage = "Owner is required")]
        [Display(Name = "Owner")]
        public Guid OwnerId { get; set; }

        public string? OwnerName { get; set; }
        public string? CurrentImagePath { get; set; }
        public string? ApprovalStatus { get; set; }

        public List<OwnerOption> AvailableOwners { get; set; } = new();
        public List<StatusOption> AvailableStatuses { get; set; } = new();
    }

    public class StatusOption
    {
        public string Value { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
}