namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingKPIData
{
    public class BookingKPIDataDto
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CompletedBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
        public decimal PendingRevenue { get; set; }
        public List<MonthlyBookingData> MonthlyBookings { get; set; } = new();
        public List<MonthlyRevenueData> MonthlyRevenue { get; set; } = new();
        public List<BookingStatusData> BookingStatusBreakdown { get; set; } = new();
        public List<PaymentStatusData> PaymentStatusBreakdown { get; set; } = new();
    }

    public class MonthlyBookingData
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class BookingStatusData
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PaymentStatusData
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
