namespace CarRentalSystem.Application.Contracts.AdminAssistant
{
    public class ChartRequest
    {
        public string ChartType { get; set; } = string.Empty; // "line", "bar", "pie", "area"
        public string DataType { get; set; } = string.Empty; // "bookings", "revenue", "customers", "cars"
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public string? GroupBy { get; set; } // "day", "week", "month", "year"
        public string? Status { get; set; }
        public Dictionary<string, object>? AdditionalFilters { get; set; }
    }
}
