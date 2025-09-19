using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class AdminAddCarViewModel
    {
        [Required(ErrorMessage = "Car name is required")]
        [Display(Name = "Car Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Car model is required")]
        [Display(Name = "Model")]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Transmission type is required")]
        [Display(Name = "Transmission")]
        public string Transmission { get; set; } = null!;

        [Required(ErrorMessage = "Fuel type is required")]
        [Display(Name = "Fuel Type")]
        public string FuelType { get; set; } = null!;

        [Required(ErrorMessage = "Rate per day is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        [Display(Name = "Rate Per Day ($)")]
        public decimal RatePerDay { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Features are required")]
        [StringLength(500, ErrorMessage = "Features cannot exceed 500 characters")]
        public string Features { get; set; } = null!;

        [Required(ErrorMessage = "Available from date is required")]
        [Display(Name = "Available From")]
        public DateTime AvailableFrom { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Available to date is required")]
        [Display(Name = "Available To")]
        public DateTime AvailableTo { get; set; } = DateTime.Now.AddMonths(6);

        [Display(Name = "Car Image")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Car owner is required")]
        [Display(Name = "Car Owner")]
        public Guid OwnerId { get; set; }

        [Display(Name = "Owner Name")]
        public string? OwnerName { get; set; }

        public List<OwnerOption> AvailableOwners { get; set; } = new();
    }

    public class OwnerOption
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
