using AutoMapper;
using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Booking;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById
{
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto?>
    {
        private readonly IBookingRepository _repository;
        private readonly IMapper _mapper;

        public GetBookingByIdQueryHandler(IBookingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var booking = await _repository.GetBookingByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return null;

            return _mapper.Map<BookingDto>(booking);
        }
    }
}
