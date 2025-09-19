using CarRentalSystem.Application.Common.Interfaces;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetPaymentStatusIdByName
{
    public class GetPaymentStatusIdByNameQueryHandler : IRequestHandler<GetPaymentStatusIdByNameQuery, Guid>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetPaymentStatusIdByNameQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Guid> Handle(GetPaymentStatusIdByNameQuery request, CancellationToken cancellationToken)
        {
            return await _bookingRepository.GetPaymentStatusIdByNameAsync(request.StatusName, cancellationToken);
        }
    }
}
