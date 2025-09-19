using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Contracts.Customer;
using CarRentalSystem.Application.Contracts.Booking;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class CarOwnerManagementViewModel
    {
        public List<CarOwnerDetailsDto> CarOwners { get; set; } = new();
        public int TotalOwners { get; set; }
        public int ActiveOwners { get; set; }
        public int InactiveOwners { get; set; }
        public decimal TotalEarnings { get; set; }
    }

    public class CarOwnerDetailsDto
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ProfileImagePath { get; set; }
        public string? NIC { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public int TotalCars { get; set; }
        public int ActiveCars { get; set; }
        public int PendingCars { get; set; }
        public int RejectedCars { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal MonthlyEarnings { get; set; }
        public string Status { get; set; } = "Active"; // Active, Inactive, Suspended
        public List<CarDto> Cars { get; set; } = new();
        public List<BookingDto> RecentBookings { get; set; } = new();
    }
}
