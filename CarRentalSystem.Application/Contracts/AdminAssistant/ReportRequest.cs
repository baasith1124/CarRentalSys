namespace CarRentalSystem.Application.Contracts.AdminAssistant
{
    public class ReportRequest
    {
        public string ReportType { get; set; } = string.Empty; // "customers", "bookings", "cars", "invoices"
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? Status { get; set; }
        public string? CustomerId { get; set; }
        public string? CarId { get; set; }
        public string? BookingId { get; set; }
        public Dictionary<string, object>? AdditionalFilters { get; set; }
    }
}
