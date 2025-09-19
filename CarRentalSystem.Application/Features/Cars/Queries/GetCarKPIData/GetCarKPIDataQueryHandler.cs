using CarRentalSystem.Application.Common.Interfaces;
using MediatR;

namespace CarRentalSystem.Application.Features.Cars.Queries.GetCarKPIData
{
    public class GetCarKPIDataQueryHandler : IRequestHandler<GetCarKPIDataQuery, CarKPIDataDto>
    {
        private readonly ICarRepository _carRepository;
        private readonly IBookingRepository _bookingRepository;

        public GetCarKPIDataQueryHandler(ICarRepository carRepository, IBookingRepository bookingRepository)
        {
            _carRepository = carRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<CarKPIDataDto> Handle(GetCarKPIDataQuery request, CancellationToken cancellationToken)
        {
            var cars = await _carRepository.GetAllCarsAsync(cancellationToken);
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);

            var totalCars = cars.Count;
            var approvedCars = cars.Count(c => c.CarApprovalStatus?.Name == "Approved");
            var pendingCars = cars.Count(c => c.CarApprovalStatus?.Name == "Pending");
            var rejectedCars = cars.Count(c => c.CarApprovalStatus?.Name == "Rejected");
            var activeCars = bookings.Select(b => b.CarId).Distinct().Count();

            var averageCarRate = cars.Any() ? cars.Average(c => c.RatePerDay) : 0;
            var totalCarValue = cars.Sum(c => c.RatePerDay * 30); // Estimated monthly value

            // Car status breakdown
            var carStatusBreakdown = cars
                .GroupBy(c => c.CarApprovalStatus?.Name ?? "Unknown")
                .Select(g => new CarStatusData
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Percentage = totalCars > 0 ? (decimal)g.Count() / totalCars * 100 : 0
                })
                .ToList();

            // Car utilization (top 10 most booked cars)
            var carUtilization = bookings
                .GroupBy(b => b.CarId)
                .Select(g => new
                {
                    CarId = g.Key,
                    BookingCount = g.Count(),
                    Revenue = g.Sum(b => b.TotalCost)
                })
                .OrderByDescending(x => x.BookingCount)
                .Take(10)
                .ToList();

            var carUtilizationData = new List<CarUtilizationData>();
            foreach (var util in carUtilization)
            {
                var car = cars.FirstOrDefault(c => c.CarId == util.CarId);
                if (car != null)
                {
                    carUtilizationData.Add(new CarUtilizationData
                    {
                        CarName = $"{car.Name} {car.Model}",
                        BookingCount = util.BookingCount,
                        Revenue = util.Revenue,
                        UtilizationRate = totalCars > 0 ? (decimal)util.BookingCount / totalCars * 100 : 0
                    });
                }
            }

            // Monthly car registrations (last 12 months) - using AvailableFrom as proxy for registration date
            var monthlyCarRegistrations = cars
                .GroupBy(c => new { c.AvailableFrom.Year, c.AvailableFrom.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Take(12)
                .Select(g => new MonthlyCarData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    NewCars = g.Count(),
                    ApprovedCars = g.Count(c => c.CarApprovalStatus?.Name == "Approved")
                })
                .ToList();

            return new CarKPIDataDto
            {
                TotalCars = totalCars,
                ApprovedCars = approvedCars,
                PendingCars = pendingCars,
                RejectedCars = rejectedCars,
                ActiveCars = activeCars,
                AverageCarRate = averageCarRate,
                TotalCarValue = totalCarValue,
                CarStatusBreakdown = carStatusBreakdown,
                CarUtilization = carUtilizationData,
                MonthlyCarRegistrations = monthlyCarRegistrations
            };
        }
    }
}
