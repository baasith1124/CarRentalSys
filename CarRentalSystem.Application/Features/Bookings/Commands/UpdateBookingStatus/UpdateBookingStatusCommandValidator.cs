using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.UpdateBookingStatus
{
    public class UpdateBookingStatusCommandValidator : AbstractValidator<UpdateBookingStatusCommand>
    {
        public UpdateBookingStatusCommandValidator()
        {
            RuleFor(x => x.BookingId).NotEmpty().WithMessage("Booking ID is required.");
            RuleFor(x => x.BookingStatusId).NotEmpty().WithMessage("Booking Status is required.");
        }
    }
}
