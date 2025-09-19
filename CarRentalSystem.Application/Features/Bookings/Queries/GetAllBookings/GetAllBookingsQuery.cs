using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetAllBookings
{
    public class GetAllBookingsQuery : IRequest<List<Booking>>
    {
    }
}
