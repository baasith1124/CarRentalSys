using CarRentalSystem.Application.Contracts.Booking;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingById
{
    public class GetBookingByIdQuery : IRequest<BookingDto?>
    {
        public Guid BookingId { get; set; }
    }
}
