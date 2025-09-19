using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetAllBookings
{
    public class GetAllBookingsQueryHandler : IRequestHandler<GetAllBookingsQuery, List<Booking>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetAllBookingsQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Booking>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.GetAllBookingsAsync(cancellationToken);
        }
    }
}
