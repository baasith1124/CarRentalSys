using CarRentalSystem.Application.Contracts.Booking;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookings
{
    public class GetBookingsQuery : IRequest<List<BookingDto>>
    {
        //  public Guid? CustomerId { get; set; }
    }
}
