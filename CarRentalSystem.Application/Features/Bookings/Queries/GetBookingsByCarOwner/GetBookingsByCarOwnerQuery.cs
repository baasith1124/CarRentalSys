using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Domain.Entities;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner
{
    public class GetBookingsByCarOwnerQuery : IRequest<List<Booking>>
    {
        public Guid CarOwnerId { get; set; }
        
        public GetBookingsByCarOwnerQuery() { }
        
        public GetBookingsByCarOwnerQuery(Guid carOwnerId)
        {
            CarOwnerId = carOwnerId;
        }
    }
}
