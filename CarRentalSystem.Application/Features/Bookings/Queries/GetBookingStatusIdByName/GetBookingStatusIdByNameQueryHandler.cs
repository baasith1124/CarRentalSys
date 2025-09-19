using CarRentalSystem.Application.Common.Interfaces;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingStatusIdByName
{
    public class GetBookingStatusIdByNameQueryHandler : IRequestHandler<GetBookingStatusIdByNameQuery, Guid>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingStatusIdByNameQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Guid> Handle(GetBookingStatusIdByNameQuery request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.GetBookingStatusIdByNameAsync(request.StatusName, cancellationToken);
        }
    }
}
