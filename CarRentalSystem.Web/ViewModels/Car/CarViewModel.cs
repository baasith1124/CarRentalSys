using System;

namespace CarRentalSystem.Web.ViewModels.Car
{
    public class CarViewModel
    {
        public Guid CarId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string GetImageFileName()
        {
            return Path.GetFileName(ImagePath);
        }

        public string FullImagePath => $"/images/cars/{GetImageFileName()}";


        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public decimal DailyRate { get; set; }
        public string Description { get; set; } = null!;
        public string Features { get; set; } = null!;
        public int Year { get; set; }
        public string FuelType { get; set; } = null!;
        public string Transmission { get; set; } = null!;

        // UI-only fields (optional)
        public string DisplayName => $"{Name} {Model}";
        public string AvailabilityRange => $"{AvailableFrom:dd MMM yyyy} - {AvailableTo:dd MMM yyyy}";
        public string FormattedRate => $"{DailyRate:C}";
    }
}
