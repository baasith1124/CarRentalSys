using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Application.Contracts.Customer;

namespace CarRentalSystem.Web.ViewModels.Dashboard
{
    public class UserDashboardViewModel
    {
        public CustomerDto Customer { get; set; } = null!;
        public List<BookingDto> RecentBookings { get; set; } = new();
        public int TotalBookings { get; set; }
        public int ActiveRentals { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal Savings { get; set; }
    }
}
