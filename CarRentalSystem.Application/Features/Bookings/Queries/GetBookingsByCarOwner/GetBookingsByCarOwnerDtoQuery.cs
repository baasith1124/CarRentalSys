using CarRentalSystem.Application.Contracts.Booking;
using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingsByCarOwner
{
    public class GetBookingsByCarOwnerDtoQuery : IRequest<List<BookingDto>>
    {
        public Guid CarOwnerId { get; set; }
        
        public GetBookingsByCarOwnerDtoQuery() { }
        
        public GetBookingsByCarOwnerDtoQuery(Guid carOwnerId)
        {
            CarOwnerId = carOwnerId;
        }
    }
}