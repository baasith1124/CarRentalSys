using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; } // Security: verify the booking belongs to this user
    }
}
