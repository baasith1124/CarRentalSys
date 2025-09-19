using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.UpdateBookingStatus
{
    public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        public UpdateBookingStatusCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
        {
            // Load the booking
            var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return false;

            // Update fields (locally)
            booking.BookingStatusId = request.BookingStatusId;
            if (request.PaymentStatusId.HasValue)
                booking.PaymentStatusId = request.PaymentStatusId.Value;

            // Save to DB
            await _bookingRepository.UpdateBookingStatusAsync(
                booking.BookingId,
                booking.BookingStatusId,
                request.PaymentStatusId,
                cancellationToken
            );

            return true;
        }
    }
}
