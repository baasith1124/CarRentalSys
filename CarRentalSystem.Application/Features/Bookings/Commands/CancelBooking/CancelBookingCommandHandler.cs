using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        public CancelBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);

            // Ensure the booking exists and belongs to the current user
            if (booking == null || booking.CustomerId != request.UserId)
                return false;

            await _bookingRepository.CancelBookingAsync(request.BookingId, cancellationToken);
            return true;
        }
    }
}
