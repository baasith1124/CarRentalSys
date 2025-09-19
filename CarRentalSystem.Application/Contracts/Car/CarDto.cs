using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Contracts.Car
{
    public class CarDto
    {
        public Guid CarId { get; set; }
        public string Name { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? ImagePath { get; set; }
        public string Description { get; set; } = null!;
        public string Features { get; set; } = null!;
        public int Year { get; set; }
        public string FuelType { get; set; } = null!;
        public string Transmission { get; set; } = null!;
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public string ApprovalStatus { get; set; } = null!;
        public decimal RatePerDay { get; set; }  
        public decimal EstimatedCost { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
        public string CarApprovalStatus { get; set; } = null!;
    }
}
