namespace CarRentalSystem.Application.Features.Customers.Queries.GetCustomerKPIData
{
    public class CustomerKPIDataDto
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int NewCustomersLastMonth { get; set; }
        public decimal CustomerGrowthRate { get; set; }
        public List<MonthlyCustomerData> MonthlyCustomerGrowth { get; set; } = new();
        public List<CustomerSegmentData> CustomerSegments { get; set; } = new();
    }

    public class MonthlyCustomerData
    {
        public string Month { get; set; } = string.Empty;
        public int NewCustomers { get; set; }
        public int TotalCustomers { get; set; }
    }

    public class CustomerSegmentData
    {
        public string Segment { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
