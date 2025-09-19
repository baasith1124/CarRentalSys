using CarRentalSystem.Application.Common.Interfaces;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingKPIData
{
    public class GetBookingKPIDataQueryHandler : IRequestHandler<GetBookingKPIDataQuery, BookingKPIDataDto>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingKPIDataQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingKPIDataDto> Handle(GetBookingKPIDataQuery request, CancellationToken cancellationToken)
        {
            var allBookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);

            // Apply date filters if provided
            var bookings = allBookings.AsQueryable();
            if (request.Year.HasValue)
            {
                bookings = bookings.Where(b => b.CreatedAt.Year == request.Year.Value);
            }

            if (request.Month.HasValue)
            {
                bookings = bookings.Where(b => b.CreatedAt.Month == request.Month.Value);
            }

            var filteredBookings = bookings.ToList();

            var totalBookings = filteredBookings.Count;
            var confirmedBookings = filteredBookings.Count(b => b.BookingStatus?.Name == "Confirmed");
            var pendingBookings = filteredBookings.Count(b => b.BookingStatus?.Name == "Pending");
            var cancelledBookings = filteredBookings.Count(b => b.BookingStatus?.Name == "Cancelled");
            var completedBookings = filteredBookings.Count(b => b.BookingStatus?.Name == "Completed");

            var totalRevenue = filteredBookings.Sum(b => b.TotalCost);
            var paidRevenue = filteredBookings.Where(b => b.PaymentStatus?.Name == "Paid").Sum(b => b.TotalCost);
            var pendingRevenue = filteredBookings.Where(b => b.PaymentStatus?.Name == "Pending").Sum(b => b.TotalCost);

            // Monthly booking data (last 12 months) - use all bookings for historical data
            var monthlyBookings = allBookings
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Take(12)
                .Select(g => new MonthlyBookingData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count()
                })
                .ToList();

            // Monthly revenue data (last 12 months) - use all bookings for historical data
            var monthlyRevenue = allBookings
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Take(12)
                .Select(g => new MonthlyRevenueData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Amount = g.Sum(b => b.TotalCost)
                })
                .ToList();

            // Booking status breakdown
            var bookingStatusBreakdown = filteredBookings
                .GroupBy(b => b.BookingStatus?.Name ?? "Unknown")
                .Select(g => new BookingStatusData
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = totalBookings > 0 ? (decimal)g.Count() / totalBookings * 100 : 0
                })
                .ToList();

            // Payment status breakdown
            var paymentStatusBreakdown = filteredBookings
                .GroupBy(b => b.PaymentStatus?.Name ?? "Unknown")
                .Select(g => new PaymentStatusData
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = totalBookings > 0 ? (decimal)g.Count() / totalBookings * 100 : 0
                })
                .ToList();

            return new BookingKPIDataDto
            {
                TotalBookings = totalBookings,
                ConfirmedBookings = confirmedBookings,
                PendingBookings = pendingBookings,
                CancelledBookings = cancelledBookings,
                CompletedBookings = completedBookings,
                TotalRevenue = totalRevenue,
                PaidRevenue = paidRevenue,
                PendingRevenue = pendingRevenue,
                MonthlyBookings = monthlyBookings,
                MonthlyRevenue = monthlyRevenue,
                BookingStatusBreakdown = bookingStatusBreakdown,
                PaymentStatusBreakdown = paymentStatusBreakdown
            };
        }
    }
}
