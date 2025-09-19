using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Domain.Entities
{
    public class Car
    {
        public Guid CarId { get; set; }
        public string Name { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? ImagePath { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }

        public string? Description { get; set; }          
        public string? Features { get; set; }             
        public int? Year { get; set; }                    
        public string? Transmission { get; set; }         
        public string? FuelType { get; set; }
        public decimal RatePerDay { get; set; }

        // Lookup
        public Guid CarApprovalStatusId { get; set; }
        public CarApprovalStatus CarApprovalStatus { get; set; } = null!;

        // Relationships
        public Guid OwnerId { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<CarDocument> Documents { get; set; } = new List<CarDocument>();
    }

}
