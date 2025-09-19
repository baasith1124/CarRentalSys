using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Booking;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner
{
    public class GetBookingsByCarOwnerDtoQueryHandler : IRequestHandler<GetBookingsByCarOwnerDtoQuery, List<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetBookingsByCarOwnerDtoQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> Handle(GetBookingsByCarOwnerDtoQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetBookingsByCarOwnerAsync(request.CarOwnerId, cancellationToken);
            return _mapper.Map<List<BookingDto>>(bookings);
        }
    }
}