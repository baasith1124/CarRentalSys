using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingKPIData
{
    public class GetBookingKPIDataQuery : IRequest<BookingKPIDataDto>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }

        public GetBookingKPIDataQuery(int? year = null, int? month = null)
        {
            Year = year;
            Month = month;
        }
    }
}
