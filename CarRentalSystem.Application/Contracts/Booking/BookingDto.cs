using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Booking
{
    public class BookingDto
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public string CarName { get; set; } = string.Empty;
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalCost { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DropLocation { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        
        // Location coordinates
        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public double? DropLatitude { get; set; }
        public double? DropLongitude { get; set; }
        
        // Additional properties for dashboard
        public CarInfo Car { get; set; } = new();
        public DateTime StartDate => PickupDate;
        public int Duration => (ReturnDate - PickupDate).Days;
        public string Status => BookingStatus;
        public decimal TotalAmount => TotalCost;
    }
    
    public class CarInfo
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }
}
