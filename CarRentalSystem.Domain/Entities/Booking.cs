using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CarRentalSystem.Domain.Entities
{
    public class Booking
    {
        public Guid BookingId { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid CarId { get; set; }
        public Car Car { get; set; } = null!;

        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal TotalCost { get; set; }

        public Guid BookingStatusId { get; set; }
        public BookingStatus BookingStatus { get; set; } = null!;

        public Guid PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Location information
        public string? PickupLocation { get; set; }
        public string? DropLocation { get; set; }
        
        // Location coordinates
        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public double? DropLatitude { get; set; }
        public double? DropLongitude { get; set; }

        public Payment? Payment { get; set; }
        public Invoice? Invoice { get; set; }
    }

}
