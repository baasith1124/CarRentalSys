using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner
{
    public class GetBookingsByCarOwnerQueryHandler : IRequestHandler<GetBookingsByCarOwnerQuery, List<Booking>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingsByCarOwnerQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Booking>> Handle(GetBookingsByCarOwnerQuery request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.GetBookingsByCarOwnerAsync(request.CarOwnerId, cancellationToken);
        }
    }
}
