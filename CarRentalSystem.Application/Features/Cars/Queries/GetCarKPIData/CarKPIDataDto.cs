namespace CarRentalSystem.Application.Features.Cars.Queries.GetCarKPIData
{
    public class CarKPIDataDto
    {
        public int TotalCars { get; set; }
        public int ApprovedCars { get; set; }
        public int PendingCars { get; set; }
        public int RejectedCars { get; set; }
        public int ActiveCars { get; set; }
        public decimal AverageCarRate { get; set; }
        public decimal TotalCarValue { get; set; }
        public List<CarStatusData> CarStatusBreakdown { get; set; } = new();
        public List<CarUtilizationData> CarUtilization { get; set; } = new();
        public List<MonthlyCarData> MonthlyCarRegistrations { get; set; } = new();
    }

    public class CarStatusData
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class CarUtilizationData
    {
        public string CarName { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal UtilizationRate { get; set; }
    }

    public class MonthlyCarData
    {
        public string Month { get; set; } = string.Empty;
        public int NewCars { get; set; }
        public int ApprovedCars { get; set; }
    }
}
