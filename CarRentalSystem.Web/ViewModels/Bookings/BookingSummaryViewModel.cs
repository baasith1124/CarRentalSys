﻿using CarRentalSystem.Application.Contracts.Car;

namespace CarRentalSystem.Web.ViewModels.Bookings
{
    public class BookingSummaryViewModel
    {
        public Guid CarId { get; set; }
        public CarDto Car { get; set; } = null!;  
        public string CarName => Car.Name;
        public string CarModel => Car.Model;
        public string ImagePath => Car.ImagePath;

        public DateTime PickupDate { get; set; }
        public DateTime DropDate { get; set; }
        public string PickupLocation { get; set; } = null!;
        public string DropLocation { get; set; } = null!;
        public int Days => Math.Max(1, (DropDate - PickupDate).Days); 
        public decimal EstimatedCost { get; set; }
        
        // Customer and KYC information
        public bool HasKYC { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        // Additional booking details
        public decimal DailyRate => Car.RatePerDay;
        public decimal ServiceFee { get; set; } = 0;
        public decimal TotalCost => (DailyRate * Days) + ServiceFee;
        
        // Driver details (for additional drivers)
        public string AdditionalDriverName { get; set; } = string.Empty;
        public string AdditionalDriverLicense { get; set; } = string.Empty;
    }
}
