using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Application.Contracts.Car;

namespace CarRentalSystem.Web.ViewModels.CarOwner
{
    public class CarOwnerDashboardViewModel
    {
        public List<CarDto> MyCars { get; set; } = new();
        public List<BookingDto> RecentBookings { get; set; } = new();
        public decimal TotalEarnings { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveCars { get; set; }
        public int PendingCars => MyCars.Count(c => c.CarApprovalStatus == "Pending");
        public int ApprovedCars => MyCars.Count(c => c.CarApprovalStatus == "Approved");
        public int RejectedCars => MyCars.Count(c => c.CarApprovalStatus == "Rejected");
    }
}
