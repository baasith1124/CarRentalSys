using CarRentalSystem.Application.Contracts.Booking;
using CarRentalSystem.Application.Contracts.Car;
using CarRentalSystem.Application.Contracts.Customer;

namespace CarRentalSystem.Web.ViewModels.Admin
{
    public class BookingManagementViewModel
    {
        public List<BookingDetailsDto> Bookings { get; set; } = new();
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public string? FilterStatus { get; set; }
        public string? FilterDateRange { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
    }

    public class BookingDetailsDto
    {
        public Guid BookingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Notes { get; set; }
        
        // Car Information
        public Guid CarId { get; set; }
        public string CarName { get; set; } = null!;
        public string CarModel { get; set; } = null!;
        public string? CarImagePath { get; set; }
        public decimal CarRatePerDay { get; set; }
        
        // Customer Information
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string? CustomerPhone { get; set; }
        
        // Car Owner Information
        public Guid CarOwnerId { get; set; }
        public string CarOwnerName { get; set; } = null!;
        public string CarOwnerEmail { get; set; } = null!;
        
        // Calculated Properties
        public int TotalDays => (int)(EndDate - StartDate).TotalDays;
        public decimal DailyRate => TotalDays > 0 ? TotalAmount / TotalDays : 0;
        public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public bool IsUpcoming => DateTime.Now < StartDate;
        public bool IsPast => DateTime.Now > EndDate;
    }
}
