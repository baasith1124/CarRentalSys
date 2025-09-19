using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Booking;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookings
{
    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, List<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public GetBookingsQueryHandler(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<List<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
            return _mapper.Map<List<BookingDto>>(bookings);
        }
    }
}
