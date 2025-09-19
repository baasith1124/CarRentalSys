using MediatR;

namespace CarRentalSystem.Application.Features.Bookings.Queries.GetBookingStatusIdByName
{
    public class GetBookingStatusIdByNameQuery : IRequest<Guid>
    {
        public string StatusName { get; set; } = null!;

        public GetBookingStatusIdByNameQuery(string statusName)
        {
            StatusName = statusName;
        }
    }
}
