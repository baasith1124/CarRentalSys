using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Contracts.KYC;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public List<CarDto> PendingCars { get; set; } = new();
        public List<KYCUploadDto> PendingKYC { get; set; } = new();
        public List<BookingDto> RecentBookings { get; set; } = new();
        public int TotalCustomers { get; set; }
        public int TotalCars => PendingCars.Count + ApprovedCars + RejectedCars;
        public int ApprovedCars { get; set; }
        public int RejectedCars { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
