using CarRentalSystem.Application.Contracts.Booking;

namespace CarRentalSystem.Web.ViewModels.CarOwner
{
    public class EarningsViewModel
    {
        public decimal TotalEarnings { get; set; }
        public decimal MonthlyEarnings { get; set; }
        public decimal PendingPayments { get; set; }
        public List<BookingDto> Bookings { get; set; } = new();
    }
}
