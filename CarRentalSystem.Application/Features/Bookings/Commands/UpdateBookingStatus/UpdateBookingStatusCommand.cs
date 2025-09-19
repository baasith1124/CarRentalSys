using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Commands.UpdateBookingStatus
{
    public class UpdateBookingStatusCommand : IRequest<bool>
    {
        public Guid BookingId { get; set; }
        public Guid BookingStatusId { get; set; }
        public Guid? PaymentStatusId { get; set; } 
    }
}
