using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Web.ViewModels.Car
{
    public class CarDetailsViewModel
    {
        public Guid CarId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty;
        public int SeatingCapacity { get; set; }
        public decimal RatePerDay { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new List<string>();

        // Computed properties
        public string FullImagePath => !string.IsNullOrEmpty(ImagePath) ? ImagePath : "~/images/default-car.jpg";
        public string AvailabilityText => $"Available from {AvailableFrom:MMM dd, yyyy} to {AvailableTo:MMM dd, yyyy}";
        public string RateText => $"${RatePerDay:F2} per day";
    }
}
