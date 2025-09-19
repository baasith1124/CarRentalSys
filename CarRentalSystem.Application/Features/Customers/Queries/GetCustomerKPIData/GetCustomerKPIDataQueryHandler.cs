using CarRentalSystem.Application.Common.Interfaces;
using MediatR;

namespace CarRentalSystem.Application.Features.Customers.Queries.GetCustomerKPIData
{
    public class GetCustomerKPIDataQueryHandler : IRequestHandler<GetCustomerKPIDataQuery, CustomerKPIDataDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;

        public GetCustomerKPIDataQueryHandler(ICustomerRepository customerRepository, IBookingRepository bookingRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<CustomerKPIDataDto> Handle(GetCustomerKPIDataQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync(cancellationToken);
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);

            var totalCustomers = customers.Count;
            var activeCustomers = bookings.Select(b => b.CustomerId).Distinct().Count();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            // Since Customer doesn't have CreatedAt, we'll use booking data to estimate customer growth
            var newCustomersThisMonth = bookings.Count(b => b.CreatedAt.Month == currentMonth && b.CreatedAt.Year == currentYear);
            var newCustomersLastMonth = bookings.Count(b => b.CreatedAt.Month == lastMonth && b.CreatedAt.Year == lastMonthYear);

            var customerGrowthRate = newCustomersLastMonth > 0 
                ? ((decimal)(newCustomersThisMonth - newCustomersLastMonth) / newCustomersLastMonth) * 100 
                : 0;

            // Monthly customer growth (last 12 months) - using booking data as proxy
            var monthlyCustomerGrowth = bookings
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Take(12)
                .Select(g => new MonthlyCustomerData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    NewCustomers = g.Select(b => b.CustomerId).Distinct().Count(),
                    TotalCustomers = totalCustomers // Use total customers as constant for now
                })
                .ToList();

            // Customer segments based on booking count
            var customerBookingCounts = bookings
                .GroupBy(b => b.CustomerId)
                .Select(g => new { CustomerId = g.Key, BookingCount = g.Count() })
                .ToList();

            var customerSegments = new List<CustomerSegmentData>
            {
                new() { Segment = "New (0 bookings)", Count = customers.Count(c => !customerBookingCounts.Any(cb => cb.CustomerId == c.Id)) },
                new() { Segment = "Occasional (1-2 bookings)", Count = customerBookingCounts.Count(cb => cb.BookingCount >= 1 && cb.BookingCount <= 2) },
                new() { Segment = "Regular (3-5 bookings)", Count = customerBookingCounts.Count(cb => cb.BookingCount >= 3 && cb.BookingCount <= 5) },
                new() { Segment = "Frequent (6+ bookings)", Count = customerBookingCounts.Count(cb => cb.BookingCount >= 6) }
            };

            // Calculate percentages
            foreach (var segment in customerSegments)
            {
                segment.Percentage = totalCustomers > 0 ? (decimal)segment.Count / totalCustomers * 100 : 0;
            }

            return new CustomerKPIDataDto
            {
                TotalCustomers = totalCustomers,
                ActiveCustomers = activeCustomers,
                NewCustomersThisMonth = newCustomersThisMonth,
                NewCustomersLastMonth = newCustomersLastMonth,
                CustomerGrowthRate = customerGrowthRate,
                MonthlyCustomerGrowth = monthlyCustomerGrowth,
                CustomerSegments = customerSegments
            };
        }
    }
}
